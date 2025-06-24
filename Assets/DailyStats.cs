[System.Serializable]
public class DailyStats
{
    public string date;
    public float earned;
    public float spent;

    public float Net => earned - spent;
}
