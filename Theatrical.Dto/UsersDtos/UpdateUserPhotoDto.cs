namespace Theatrical.Dto.UsersDtos;

public class UpdateUserPhotoDto
{
    public string Photo { get; set; }
    public string? Label { get; set; }
    public bool IsProfile { get; set; }
}