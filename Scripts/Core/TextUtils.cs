

using System.Text;

public static class TextUtils
{
    public static string TimeToString(float time)
    {
        StringBuilder sb = new();

        int mins = (int)(time / 60f);
        if (mins > 0)
        {
            time -= mins * 60;
            sb.Append($"{mins} : ");
        }
        
        sb.Append((int)time);
        
        return sb.ToString();
    }
}