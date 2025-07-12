public static class AvatarHelper
{
    public static string GravatarUrl(string email, int size = 40)
    {
        var md5 = System.Security.Cryptography.MD5.HashData(
                      System.Text.Encoding.UTF8.GetBytes(email.Trim().ToLower()));
        var hash = string.Concat(md5.Select(b => b.ToString("x2")));
        return $"https://www.gravatar.com/avatar/{hash}?s={size}&d=identicon";
    }
}

