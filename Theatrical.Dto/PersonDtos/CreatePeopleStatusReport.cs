namespace Theatrical.Dto.PersonDtos;

public class CreatePeopleStatusReport
{
    public int AddedPeople { get; set; }
    public int AlreadyExistingPeople { get; set; }
    public int NullNameNotAddedPeople { get; set; }
}