using BFC.WebTest.BackgroundWorker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BFC.WebTest.Services
{
    public interface IStudentService
    {
        bool SaveStudent(string name);
    }
}
