using System.ComponentModel.DataAnnotations;

namespace WtSbAssistant.Core.DataAccess.DatabaseAccess.Entities
{
    public class WtBattleAction
    {
        [Key]
        public int UniqueId { get; set; }

        public int VehicleId { get; set; }

        public int PlayerId { get; set; }

        public int MatchId { get; set; }

        public int? LinkedActionId { get; set; }

        public WtBattleActionType ActionType { get; set; }

        public virtual WtMatch Match { get; set; } = null!;

        public virtual WtPlayer Player { get; set; } = null!;

        public virtual WtVehicle Vehicle { get; set; } = null!;

        public virtual WtBattleAction? LinkedAction { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is WtBattleAction battleAction &&
                   VehicleId == battleAction.VehicleId &&
                   PlayerId == battleAction.PlayerId &&
                   MatchId == battleAction.MatchId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(VehicleId, PlayerId, MatchId);
        }
    }
}
