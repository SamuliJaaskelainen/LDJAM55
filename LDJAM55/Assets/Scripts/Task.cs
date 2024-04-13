namespace DataTypes
{
    public class Task
    {
        enum Type
        {
            Feature,
            Bug
        }

        Type type;
        int cost;
    }
}