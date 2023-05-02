public class FinishCarInfo
{
    public CarName carName;

    public int position;
    public int completeTime;

    public bool isPlayer;

    public FinishCarInfo(CarName carName, int position, int completeTime, bool isPlayer)
    {
        this.carName = carName;
        this.position = position;
        this.completeTime = completeTime;
        this.isPlayer = isPlayer;
    }
}
