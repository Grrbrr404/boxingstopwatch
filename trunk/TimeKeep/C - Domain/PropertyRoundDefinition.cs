namespace TimeKeep.Domain {
    using TimeKeep.Domain.Interfaces;

    class PropertyRoundDefinition : IRoundDefinition {
    public int GetMaxRounds() {
      return Properties.Settings.Default.MaxRounds;
    }

    public int GetRoundTimeInSeconds() {
      return Properties.Settings.Default.RoundInSeconds;
    }

    public int GetPauseTimeInSeconds() {
      return Properties.Settings.Default.PauseInSeconds;
    }
  }
}
