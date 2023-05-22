using Ardalis.Specification;

namespace OregonNexus.Broker.Domain.Specifications;

public class UserRolesByUserSpec : Specification<UserRole>
{
  public UserRolesByUserSpec(Guid UserId)
  {
    Query
        .Where(user => user.UserId == UserId);
  }
}