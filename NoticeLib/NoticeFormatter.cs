namespace NoticeLib
{
    public static class NoticeFormatter
    {
        public static string FormatNoticeText(NoticeData notice, string systemInfo)
        {
            return $"==========================================\n" +
                   $"  [{notice.Version}] {notice.Title}\n" +
                   $"  작성일: {notice.Date}\n" +
                   $"==========================================\n\n" +
                   $"{notice.Content}\n\n" +
                   $"------------------------------------------\n" +
                   $"{systemInfo}\n" +
                   $"==========================================";
        }
    }
}