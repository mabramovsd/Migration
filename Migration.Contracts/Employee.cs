using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migration.Contracts
{
    /// <summary>
    /// Abstract class for employee
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// Employee Identifier (unique)
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Employee name
        /// </summary>
        public string FullName { get; set; }
    }
}
