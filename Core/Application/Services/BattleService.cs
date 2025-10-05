using Habit_Battles.Core.Application.Interfaces.Repositories;
using Habit_Battles.Core.Application.Interfaces.Services;
using Habit_Battles.Core.Domain.Entities;
using Habit_Battles.Core.Domain.Enums;
using Habit_Battles.Models;
using Habit_Battles.Models.BattleModel;

namespace Habit_Battles.Core.Application.Services
{
    public class BattleService : IBattleService
    {
        private readonly IBattleRepository _battlerepo;
        private readonly IUserRepository _userRepository;
        private readonly IStrikeRepository _strikeRepository;

        public BattleService(IBattleRepository battleRepository, IUserRepository userRepository, IStrikeRepository strikeRepository)
        {
            _battlerepo = battleRepository;
            _userRepository = userRepository;
            _strikeRepository = strikeRepository;
        }
        public async Task<BaseResponse<BattleAcceptResponse>> AcceptBattle(int battleId, int opponentId)
        {
            var battle = await _battlerepo.GetAsync(battleId);
            if (battle == null || battle.OpponentId != opponentId)
            {
                return new BaseResponse<BattleAcceptResponse>
                {
                    IsSuccessful = false,
                    Message = "Theres no active battle for this user",
                    Value = new BattleAcceptResponse{}
                };
            }
            ;
            battle.Status = Domain.Enums.Status.Active;
            _battlerepo.Update(battle);

            return new BaseResponse<BattleAcceptResponse>
            {
                IsSuccessful = true,
                Message = "Your battle is now active",
                Value = new BattleAcceptResponse
                {
                     Id = battleId,
                     Status = battle.Status,
                }
            };
        }
        public async Task<BaseResponse<BattleResponse>> CreateBattle(BattleRequest request, int userId)
        {
            var user = await _userRepository.GetAsync(userId);
            if(user.Id == request.opponentId)
            {
                return new BaseResponse<BattleResponse>
                {
                    Message = "You cannot compete with yourself",
                    IsSuccessful = false,
                    Value = new BattleResponse{}
                };
            }
            ;
            var battle = new Battle
            {
                Habit = request.Habit,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(request.duration),
                DurationDays = request.duration,
                MonsterType = MonsterType.default_monster,
                Status = Domain.Enums.Status.Pending,
                OpponentId = request.opponentId,
                OpponentHealth = 100,
                CreatorHealth = 100,
                CreatedBy = userId.ToString(),
            };
            var response = await _battlerepo.AddAsync(battle);
            return new BaseResponse<BattleResponse>
            {
                Message = "Battle Successfully Created and Pending",
                IsSuccessful = true,
                Value = new BattleResponse
                {
                    EndDate = response.EndDate,
                    Id = response.Id,
                    Status = response.Status,
                    StartDate = response.StartDate,
                    DurationDays = response.DurationDays,
                    MonsterType = response.MonsterType,
                    CreatorHealth = response.CreatorHealth,
                    Habit = response.Habit,
                    OpponentHealth = response.OpponentHealth,
                }
            };
        }
        public async Task<BaseResponse<BattleResponse?>> GetBattleStatusAsync(int battleId)
        {
            var battle = await _battlerepo.GetAsync(battleId);
            if (battle == null)
            {
                return new BaseResponse<BattleResponse?>
                {
                    IsSuccessful = false,
                    Message = "No battle with this Id",
                    Value = new BattleResponse { }
                };
            };

            var creatorId = int.Parse(battle.CreatedBy);
            var creator = await _userRepository.GetAsync(creatorId);
            var opponent = await _userRepository.GetAsync(battle.OpponentId);

            async Task<int> CalculateStreakAsync(int userId)
            {
                var strikes = (await _strikeRepository.GetByBattleAndUserAsync(battleId, userId))
                                .Select(s => s.Date.Date)
                                .Distinct()
                                .OrderByDescending(d => d)
                                .ToList();

                int streak = 0;
                var day = DateTime.UtcNow.Date;
                while (strikes.Contains(day))
                {
                    streak++;
                    day = day.AddDays(-1);
                }
                return streak;
            }

            var creatorStreak = await CalculateStreakAsync(creatorId);
            var opponentStreak = await CalculateStreakAsync(battle.OpponentId);

            return new BaseResponse<BattleResponse?>
            {
                 IsSuccessful = true,
                  Message = "Battle successfully retrieved",
                  Value = new BattleResponse
                  {
                      Id = battle.Id,
                      Habit = battle.Habit,
                      Status = battle.Status,
                      StartDate = battle.StartDate,
                      EndDate = battle.EndDate,
                      DurationDays = battle.DurationDays,
                      CreatorHealth = battle.CreatorHealth,
                      OpponentHealth = battle.OpponentHealth,
                      Players = new List<PlayerResponse>
                      {
                          new PlayerResponse
                          {
                                UserId = creator.Id,
                                Username = creator.Username,
                                Streak = creatorStreak
                          },
                          new PlayerResponse
                          {
                               UserId = opponent.Id,
                               Username = opponent.Username,
                               Streak = opponentStreak
                          }
                      }
                  }
            };
        }
        public async Task<BaseResponse<EndBattleResponse>> EndBattleAsync(int battleId, int winnerId)
        {
            var battle = await _battlerepo.GetAsync(battleId);
            if (battle == null)
                return new BaseResponse<EndBattleResponse>
                {
                    IsSuccessful = false,
                    Message = "Battle not found"
                };

            int.TryParse(battle.CreatedBy, out var creatorId);
            var loserId = creatorId == winnerId ? battle.OpponentId : creatorId;

            battle.Status = Status.Completed;
            battle.WinnerId = winnerId;
            _battlerepo.Update(battle);

            var winner = await _userRepository.GetAsync(winnerId);
            if (winner != null)
            {
                winner.Coins += 50;
                _userRepository.Update(winner);
            }


            return new BaseResponse<EndBattleResponse>
            {
                IsSuccessful = true,
                Message = "Battle ended successfully",
                Value = new EndBattleResponse
                {
                    BatlleId = battle.Id,
                    Winner = winner?.Username ?? "Unknown",
                    Loser = loserId.ToString(),
                    CoinsAwarded = 50
                }
            };
        }
        public async Task<BaseResponse<IEnumerable<LeaderBoardResponse>>> GetLeaderBoardAsync()
        {
            var users = await _userRepository.GetAllAsync();
            if(users == null)
            {
                return new BaseResponse<IEnumerable<LeaderBoardResponse>>
                {
                    IsSuccessful = false,
                    Message = "No Active Users",
                };
            }

            var battles = await _battlerepo.GetAllAsync();
            if (battles == null)
            {
                return new BaseResponse<IEnumerable<LeaderBoardResponse>>
                {
                    IsSuccessful = false,
                    Message = "No Active Battle",
                };
            }

            var leaderboard = users.Select(user =>
            {
                var wins = battles.Count(b => b.Status == Domain.Enums.Status.Completed &&
                                              b.Id == user.Id);

                return new LeaderBoardResponse
                {
                     UserName = user.Username,
                    Wins = wins,
                    Coins = user.Coins
                };
            })
            .OrderByDescending(x => x.Wins)
            .ThenByDescending(x => x.Coins)
            .ToList();

            return new BaseResponse<IEnumerable<LeaderBoardResponse>>
            {
                IsSuccessful = true,
                Message = "Leaderboard retrieved successfully",
                Value = leaderboard
            };
        }
        public async Task<BaseResponse<object>> HandleStrikeAsync(int battleId, int userId, bool isRealTime = false, bool isSuccess = true)
        {
            var battle = await _battlerepo.GetAsync(battleId);
            if (battle == null)
                return new BaseResponse<object> 
                { 
                    IsSuccessful = false, 
                    Message = "Battle not found" 
                };

            if (battle.Status != Status.Active)
                return new BaseResponse<object> 
                { 
                    IsSuccessful = false, 
                    Message = "Battle not active" 
                };

            int.TryParse(battle.CreatedBy, out var creatorId);
            var opponentId = battle.OpponentId;

            if (!isRealTime)
            {
                if (await _strikeRepository.HasStrikeForTodayAsync(battleId, userId))
                    throw new InvalidOperationException("Strike already logged for today.");

                var strike = new Strike
                {
                    BattleId = battleId,
                    UserId = userId,
                    Date = DateTime.UtcNow.Date
                };

                await _strikeRepository.AddAsync(strike);
            }

            if (isRealTime ? isSuccess : true)
            {
                const int damage = 20;

                if (userId == creatorId)
                {
                    battle.OpponentHealth = Math.Max(0, battle.OpponentHealth - damage);
                    if (battle.OpponentHealth <= 0)
                    {
                        var result = await EndBattleAsync(battleId, creatorId);
                        return new BaseResponse<object>
                        {
                            IsSuccessful = result.IsSuccessful,
                            Message = "Youve used all your lives the game has ended",
                            Value = result.Value 
                        };
                    }
                        

                }
                else if (userId == opponentId)
                {
                    battle.CreatorHealth = Math.Max(0, battle.CreatorHealth - damage);
                    if (battle.CreatorHealth <= 0)
                    {
                        var result = await EndBattleAsync(battleId, opponentId);
                        return new BaseResponse<object>
                        {
                            IsSuccessful = result.IsSuccessful,
                            Message = "Youve used all your lives the game has ended",
                            Value = result.Value
                        };
                    }
                }
            }

            _battlerepo.Update(battle);

            if (!isRealTime)
            {
                var allStrikes = (await _strikeRepository.GetByBattleAndUserAsync(battleId, userId))
                                    .Select(s => s.Date.Date)
                                    .Distinct()
                                    .OrderByDescending(d => d)
                                    .ToList();

                int streak = 0;
                var day = DateTime.UtcNow.Date;
                while (allStrikes.Contains(day))
                {
                    streak++;
                    day = day.AddDays(-1);
                }

                const int CoinsPerStrike = 10;
                var user = await _userRepository.GetAsync(userId);
                if (user != null)
                {
                    user.Coins += CoinsPerStrike;
                    _userRepository.Update(user);
                }

                return new BaseResponse<object>
                {
                    IsSuccessful = true,
                    Message = "Strike logged successfully",
                    Value = new StrikeResponse
                    {
                        BattleId = battle.Id,
                        CreatorHealth = battle.CreatorHealth,
                        OpponentHealth = battle.OpponentHealth,
                        Streak = streak,
                        CoinEarned = CoinsPerStrike,
                        UserId = userId
                    }
                };
            }

            return new BaseResponse<object>
            {
                IsSuccessful = true,
                Message = isSuccess ? "Attack landed!" : "Attack missed.",
                Value = new EndBattleResponse
                {
                    BatlleId = battle.Id,
                    Winner = "",
                    Loser = "",
                    CoinsAwarded = 0,
                    CreatorHealth = battle.CreatorHealth,
                    OpponentHealth = battle.OpponentHealth
                }
            };
        }
        public async Task<BaseResponse<object>> ProcessDailyResultsAsync()
        {
            var battles = await _battlerepo.GetActiveBattlesAsync();
            if (battles == null)
            {
                return new BaseResponse<object>
                {
                    IsSuccessful = false,
                    Message = "No Active Battle"
                };
            }

            foreach (var battle in battles)
            {
                int.TryParse(battle.CreatedBy, out var creatorId);
                var opponentId = battle.OpponentId;

                var creatorStruck = await _strikeRepository.HasStrikeForTodayAsync(battle.Id, creatorId);
                var opponentStruck = await _strikeRepository.HasStrikeForTodayAsync(battle.Id, opponentId);

                const int monsterAttack = 15;
                const int monsterHeal = 10;

                if (!creatorStruck && opponentStruck)
                {
                    battle.CreatorHealth = Math.Max(0, battle.CreatorHealth - monsterAttack);
                }
                else if (!opponentStruck && creatorStruck)
                {
                    battle.OpponentHealth = Math.Max(0, battle.OpponentHealth - monsterAttack);
                }
                else if (!creatorStruck && !opponentStruck)
                {
                    battle.CreatorHealth = Math.Min(100, battle.CreatorHealth + monsterHeal);
                    battle.OpponentHealth = Math.Min(100, battle.OpponentHealth + monsterHeal);
                }

                if (battle.CreatorHealth <= 0)
                    await EndBattleAsync(battle.Id, opponentId);

                else if (battle.OpponentHealth <= 0)
                    await EndBattleAsync(battle.Id, creatorId);

                else
                    _battlerepo.Update(battle);
            }

            return new BaseResponse<object>
            {
                IsSuccessful = true,
                Message = "Daily battle results processed successfully."
            };
        }
        public async Task<BaseResponse<IEnumerable<BattleResponse>>> GetActiveBattlesAsync()
        {
            var activeBattles = await _battlerepo.GetActiveBattlesAsync();
            if(activeBattles == null)
            {
                return new BaseResponse<IEnumerable<BattleResponse>>
                {
                    IsSuccessful = false,
                    Message = "Active battles retrieved successfully",
                };
            }

            var response = activeBattles.Select(b => new BattleResponse
            {
                Id = b.Id,
                Habit = b.Habit,
                Status = b.Status,
                StartDate = b.StartDate,
                EndDate = b.EndDate,
                DurationDays = b.DurationDays,
                CreatorHealth = b.CreatorHealth,
                OpponentHealth = b.OpponentHealth
            });

            return new BaseResponse<IEnumerable<BattleResponse>>
            {
                IsSuccessful = true,
                Message = "Active battles retrieved successfully",
                Value = response
            };
        }
        public async Task<BaseResponse<object>> ProcessDailyResultsForUserAsync(int userId)
        {
            var battles = await _battlerepo.GetActiveBattlesByUserAsync(userId);
            if(battles == null)
            {
                return new BaseResponse<object>
                {
                    IsSuccessful = false,
                    Message = "Daily results not processed for this user."
                };
            }

            foreach (var battle in battles)
            {
                int.TryParse(battle.CreatedBy, out var creatorId);
                var opponentId = battle.OpponentId;

                var creatorStruck = await _strikeRepository.HasStrikeForTodayAsync(battle.Id, creatorId);
                var opponentStruck = await _strikeRepository.HasStrikeForTodayAsync(battle.Id, opponentId);

                const int monsterAttack = 15;
                const int monsterHeal = 10;

                if (!creatorStruck && opponentStruck)
                    battle.CreatorHealth = Math.Max(0, battle.CreatorHealth - monsterAttack);
                else if (!opponentStruck && creatorStruck)
                    battle.OpponentHealth = Math.Max(0, battle.OpponentHealth - monsterAttack);
                else if (!creatorStruck && !opponentStruck)
                {
                    battle.CreatorHealth = Math.Min(100, battle.CreatorHealth + monsterHeal);
                    battle.OpponentHealth = Math.Min(100, battle.OpponentHealth + monsterHeal);
                }

                if (battle.CreatorHealth <= 0)
                    await EndBattleAsync(battle.Id, opponentId);
                else if (battle.OpponentHealth <= 0)
                    await EndBattleAsync(battle.Id, creatorId);
                else
                    _battlerepo.Update(battle);
            }

            return new BaseResponse<object>
            {
                IsSuccessful = true,
                Message = "Daily results processed for this user."
            };
        }

    }
}
