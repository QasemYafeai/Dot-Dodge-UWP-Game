using System.Collections.Generic;
using System.IO;

public class HighScoreManager
{
    private const string HighScoreFileName = "highscores.txt";

    public List<int> ReadHighScores()
    {
        List<int> highScores = new List<int>();

        
            if (File.Exists(HighScoreFileName))
            {
                string[] lines = File.ReadAllLines(HighScoreFileName);
                foreach (string line in lines)
                {
                    if (int.TryParse(line, out int score))
                    {
                        highScores.Add(score);
                    }
                }
            }
       
        return highScores;
    }

    public void WriteHighScores(List<int> highScores)
    {
            using (StreamWriter writer = File.AppendText(HighScoreFileName))
            {
                foreach (int score in highScores)
                {
                    writer.WriteLine(score);
                }
            }
        }
}
