using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migration.Contracts
{
    /// <summary>
    /// Interface for employee
    /// </summary>
    public interface IEmployee
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
