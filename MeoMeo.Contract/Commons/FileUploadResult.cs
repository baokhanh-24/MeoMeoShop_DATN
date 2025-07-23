namespace MeoMeo.Contract.Commons;

public class FileUploadResult
{
    public string FileName { get; set; }
    public string RelativePath { get; set; }
    public string FullPath { get; set; } 
    public int? FileType { get; set; }  
    public long Size { get; set; } 
}