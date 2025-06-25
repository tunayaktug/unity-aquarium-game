[System.Serializable]
public class DailyStats
{
    public float earned;
    public float spent;
    public int fishSold;
    public int fishFed;
    public string date;
    public float Net => earned - spent;

    public void Reset()
    {
        earned = 0;
        spent = 0;
        fishSold = 0;
        fishFed = 0;
    }
}
