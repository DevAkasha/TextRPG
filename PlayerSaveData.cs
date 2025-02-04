
using static Program;

internal class PlayerSaveData
{
    public int Level { get; set; }
    public int Exp { get; set; }
    public string Name { get; set; }
    public JobType JobType { get; set; }
    public int Attack { get; set; }
    public int AdditionalAttack { get; set; }
    public int Defence { get; set; }
    public int AdditionalDefence { get; set; }
    public int Health { get; set; }
    public int Gold { get; set; }
    public int[,] ItemSaveInfo { get; set; }
}