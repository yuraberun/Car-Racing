public class FinishedCarInfo
{
    public CarName carName;

    public int position;
    public int completeTime;

    public bool isPlayer;

    public FinishedCarInfo(CarName carName, int position, int completeTime, bool isPlayer)
    {
        this.carName = carName;
        this.position = position;
        this.completeTime = completeTime;
        this.isPlayer = isPlayer;
    }
}
