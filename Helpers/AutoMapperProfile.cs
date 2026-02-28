using AutoMapper;
using CRUDWithAuth.Models.DTO;
using CRUDWithAuth.Models.Expense;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CRUDWithAuth.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ExpenseDetails, ExpenseResponseDTO>();

        }
    }
}
