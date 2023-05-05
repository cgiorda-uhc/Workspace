using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VCWebPortal.API.Data;
using VCWebPortal.API.Models;

namespace VCWebPortal.API.Controllers
{

    //https://superuser.com/questions/346372/how-do-i-know-what-proxy-server-im-using
    /*
     ping wpad.ms.ds.uhc.com


10.97.164.144

npm config set registry http://registry.npmjs.org/

npm config set proxy http://149.111.199.54:8080
npm config set https-proxy http://149.111.199.54:8080
     
     */


    [ApiController]
    [Route("api/[controller]")]
    public class VCWebPortalTestController : Controller
    {
        private readonly VCWebPortalTestDbContext _vcWebPortalTestDbContext;
        //INJECTED CONTEXT IN Program.cs = builder.Services.AddDbContext<VCWebPortalTestDbContext>
        public VCWebPortalTestController(VCWebPortalTestDbContext vcWebPortalTestDbContext)
        {
            _vcWebPortalTestDbContext = vcWebPortalTestDbContext;
        }

        //GET ALL VCWebPortalTests
        //GET: api/VCWebPortalTest
        [HttpGet]
        public async Task<IActionResult> GetAllVCWebPortalTests()
        {
           var vcWebPortalTests = await _vcWebPortalTestDbContext.VCWebPortalTests.ToListAsync();
            return Ok(vcWebPortalTests);
        }


        //GET ALL VCWebPortalTests
        [HttpGet]
        [Route("{id:guid}")]
        [ActionName("GetVCWebPortalTest")]
        public async Task<IActionResult> GetVCWebPortalTest([FromRoute] Guid id)
        {
            var vcWebPortalTest = await _vcWebPortalTestDbContext.VCWebPortalTests.FirstOrDefaultAsync(x => x.id == id);
            if(vcWebPortalTest != null)
            {
                return Ok(vcWebPortalTest);
            }

            return NotFound("vcWebPortalTest not found");

        }


        //ADD NEW VCWebPortalTest
        [HttpPost]
        public async Task<IActionResult> AddVCWebPortalTest([FromBody] VCWebPortalTest vcWebPortalTest)
        {
            vcWebPortalTest.id = Guid.NewGuid();
            await _vcWebPortalTestDbContext.VCWebPortalTests.AddAsync(vcWebPortalTest);
            await _vcWebPortalTestDbContext.SaveChangesAsync();

            //CreatedAtAction RETURNS 201 RESPONSE AND ADDS HEADER TO RESPONSE
            return CreatedAtAction(nameof(GetVCWebPortalTest),new { id = vcWebPortalTest.id }, vcWebPortalTest);

        }

        //UPDATE EXISTING VCWebPortalTest
        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateVCWebPortalTest([FromRoute] Guid id, [FromBody] VCWebPortalTest vcWebPortalTest)
        {
            var existingVCWebPortalTest = await _vcWebPortalTestDbContext.VCWebPortalTests.FirstOrDefaultAsync(x => x.id == id);
            if (existingVCWebPortalTest != null)
            {
                existingVCWebPortalTest.CardholderName = vcWebPortalTest.CardholderName;
                existingVCWebPortalTest.CardNumber = vcWebPortalTest.CardNumber;
                existingVCWebPortalTest.ExpiryMonth = vcWebPortalTest.ExpiryMonth;
                existingVCWebPortalTest.ExpiryYear = vcWebPortalTest.ExpiryYear;
                existingVCWebPortalTest.CVC = vcWebPortalTest.CVC;

                await _vcWebPortalTestDbContext.SaveChangesAsync();
                return Ok(existingVCWebPortalTest);

            }
            return NotFound("vcWebPortalTest not found");
        }


        //UPDATE EXISTING VCWebPortalTest
        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeletVCWebPortalTest([FromRoute] Guid id, [FromBody] VCWebPortalTest vcWebPortalTest)
        {
            var existingVCWebPortalTest = await _vcWebPortalTestDbContext.VCWebPortalTests.FirstOrDefaultAsync(x => x.id == id);
            if (existingVCWebPortalTest != null)
            {
                _vcWebPortalTestDbContext.Remove(existingVCWebPortalTest);
                await _vcWebPortalTestDbContext.SaveChangesAsync();
                return Ok(existingVCWebPortalTest);

            }
            return NotFound("vcWebPortalTest not found");
        }

    }
}
