namespace TimeKeep.Domain.RoundManager {
  public interface IRoundDefinition {
    
    /// <summary>
    /// Determines how many rounds should be executed. Return 0 for infinite rounds.
    /// </summary>
    /// <returns></returns>
    int GetMaxRounds();

    /// <summary>
    /// Determines the length of one round in seconds.
    /// </summary>
    /// <returns></returns>
    int GetRoundTimeInSeconds();

    /// <summary>
    /// Determines the length of a pause between two rounds.
    /// </summary>
    /// <returns></returns>
    int GetPauseTimeInSeconds();
  }
}