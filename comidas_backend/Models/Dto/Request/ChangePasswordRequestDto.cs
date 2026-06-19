namespace comidas_backend.Models.Dto.Request;

public class ChangePasswordRequestDto
{
    public string oldPassword { get; set; }
    public string newPassword { get; set; }
    public string newPasswordRepeat { get; set; }
}