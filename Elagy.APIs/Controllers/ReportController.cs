using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReportProPDF.Services;
using ReportProPDF.Models;
using ReportProPDF.IServices;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Claims;
using System.Threading.Tasks;
namespace Elagy.APIs.Controllers
{

   
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportPro _reportProPDFService;
        public ReportController(IReportPro reportService)
        {
            _reportProPDFService = reportService;
        }


        [HttpGet("GetReport")]
        [Produces("application/pdf")]
        public async Task<IActionResult> Get()
        {

            List<dynamic> data = new List<dynamic>
            {
               
                new { DepartmentName = "HR", BranchName = "Branch Office", EmployeeName = "Jane Smith", Salary = 60000 },
                new { DepartmentName = "Finance", BranchName = "Head Office", EmployeeName = "Alice Johnson", Salary = 70000 },
                new { DepartmentName = "IT", BranchName = "Branch Office", EmployeeName = "Bob Brown", Salary = 80000 }
            };
            try 
            {
                var model = new ReportProPDFModel
                {
                    Title1 = "Company Name Inc.",
                    Title2 = "Annual Employee Report",
                    Title3 = "Fiscal Year 2024",
                    ReportName = "Employee Salary Report",
                   // LogoPath = "logos/company-logo.jpg", // Path in wwwroot
                    ColumnsName = new List<string> { "Department", "Branch", "Employee", "Salary" },
                    ColumnsSize = new List<int> { 3, 3, 4, 2 }, // Relative column widths
                    PrintedBy = $"Ziyad - Admin",
                    TotalName = $"Total Employees: {3} | Total Salary: {210000:C}",
                    DataModel = data
                    
                    

                };
                var result= await _reportProPDFService.GenerateReportPro(model);
                return File(result.pdf,result.Octet,result.FileName);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }
    }
}
