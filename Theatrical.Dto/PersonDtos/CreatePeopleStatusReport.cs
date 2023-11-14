namespace Theatrical.Dto.PersonDtos;

public class CreatePeopleStatusReport
{
    public int AddedPeople { get; set; }
    public int AlreadyExistingButUpdatedPeople { get; set; }
    public int NullNameNotAddedPeople { get; set; }
}