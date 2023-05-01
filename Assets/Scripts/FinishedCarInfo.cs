public class FinishedCarInfo
{
    public int position;
    public int completeTime;

    public bool isPlayer;

    public FinishedCarInfo(int position, int completeTime, bool isPlayer)
    {
        this.position = position;
        this.completeTime = completeTime;
        this.isPlayer = isPlayer;
    }
}
