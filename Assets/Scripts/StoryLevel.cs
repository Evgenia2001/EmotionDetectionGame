public class StoryLevel
{
    public string name;
    public string lvlDescription;
    public string sadAnswer;
    public string neutralAnswer;
    public string surprisedAnswer;
    public StoryLevel sad;
    public StoryLevel neutral;
    public StoryLevel surprised;
    public int emotion = 0;
    public int enemyCount = 0;

    public bool increaseEnemies;
    public int foodAmount = 2;
    public bool companion;
    public StoryLevel(string d)
    {
        lvlDescription = d;
        sad = null;
        neutral = null;
        surprised = null;
    }

    public void setAnswers(string sad, string neutral, string surprised)
    {
        sadAnswer = sad;
        neutralAnswer = neutral;
        surprisedAnswer = surprised;
    }
}