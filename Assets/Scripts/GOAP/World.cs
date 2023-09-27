
// Notion of this script is originated from https://learn.unity.com/tutorial/the-world-states?uv=2019.4&courseId=5dd851beedbc2a1bf7b72bed&projectId=5e0bc1a5edbc2a035d136397
public sealed class World
{
    private static readonly World instance = new World();
    private static CatStates cat;

    static World()
    {
        cat = new CatStates();

    }

    private World()
    {

    }

    public static World Instance
    {
        get { return instance; }
    }
    public CatStates GetCat()
    {
        return cat;
    }

}
