using AA.WebApi.Interfaces;

namespace AA.WebApi.Repositories
{
    public class DummyRepository : IDummyRepository
    {
        public string GetName()
        {
            return "Arda";
        }
    }
}
