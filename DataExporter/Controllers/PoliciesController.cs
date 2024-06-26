﻿using DataExporter.Dtos;
using DataExporter.Services;
using Microsoft.AspNetCore.Mvc;

namespace DataExporter.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PoliciesController : ControllerBase
    {
        private PolicyService _policyService;

        public PoliciesController(PolicyService policyService) 
        { 
            _policyService = policyService;
        }

        [HttpPost]
        public async Task<IActionResult> PostPolicies([FromBody]CreatePolicyDto createPolicyDto)
        {
            var readPolicyDto = await _policyService.CreatePolicyAsync(createPolicyDto);

            if (readPolicyDto == null)
            {
                return StatusCode(500);
            }

            return Ok(readPolicyDto);
        }


        [HttpGet]
        public async Task<IActionResult> GetPolicies()
        {
            //Null or ZeroCount check can be added here if necessary

            return Ok(await _policyService.ReadPoliciesAsync());
        }

        [HttpGet("{policyId}")]
        public async Task<IActionResult> GetPolicy(int policyId)
        {
            var result = await _policyService.ReadPolicyAsync(policyId);

            if (result == null)
            {
                return NotFound("There is no policy with such ID.");
            }

            return Ok(result);
        }


        [HttpPost("export")]
        public async Task<IActionResult> ExportData([FromQuery]DateTime startDate, [FromQuery] DateTime endDate)
        {
            var exportDtos = await _policyService.ExportPoliciesDataByDate(startDate, endDate);

            //Null or ZeroCount check can be added here if necessary

            return Ok(exportDtos);
        }
    }
}
