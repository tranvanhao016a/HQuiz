using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorQuiz.Shared.DTO
{
    public record PagedResult<TRecord>(TRecord[] Records, int TotalCount);


}
