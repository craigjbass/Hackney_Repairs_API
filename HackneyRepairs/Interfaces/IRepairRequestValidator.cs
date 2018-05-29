using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HackneyRepairs.Models;
using HackneyRepairs.Validators;

namespace HackneyRepairs.Interfaces
{
    public interface IRepairRequestValidator
    {
        RepairRequestValidationResult Validate(RepairRequest request);
    }
}
