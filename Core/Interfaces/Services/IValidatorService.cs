using Core.Common;
using Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Services
{
    public interface IValidatorService
    {
        Task<Response<string>> ValidateCoursePlanning(int Courseid);

    }
}
