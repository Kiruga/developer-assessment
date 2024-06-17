using DataExporter.Dtos;
using DataExporter.Model;
using Microsoft.EntityFrameworkCore;


namespace DataExporter.Services
{
    public class PolicyService
    {
        private ExporterDbContext _dbContext;

        public PolicyService(ExporterDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbContext.Database.EnsureCreated();
        }

        /// <summary>
        /// Creates a new policy from the DTO.
        /// </summary>
        /// <param name="policy"></param>
        /// <returns>Returns a ReadPolicyDto representing the new policy, if succeded. Returns null, otherwise.</returns>
        public async Task<ReadPolicyDto?> CreatePolicyAsync(CreatePolicyDto createPolicyDto)
        {
            var policy = new Model.Policy
            {
                PolicyNumber = createPolicyDto.PolicyNumber,
                Premium = createPolicyDto.Premium,
                StartDate = createPolicyDto.StartDate
            };

            try
            {
                await _dbContext.Policies.AddAsync(policy);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                //Just an example of handling errors here, there are other options:
                //- Another exception can be thrown
                //- A new Result class can be created
                return new ReadPolicyDto();
            }

            var policyReadDto = new ReadPolicyDto
            {
                Id = policy.Id,
                PolicyNumber = policy.PolicyNumber,
                Premium = policy.Premium,
                StartDate = policy.StartDate
            };

            return await Task.FromResult(policyReadDto);
        }

        /// <summary>
        /// Retrives all policies.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns a list of ReadPoliciesDto.</returns>
        public async Task<IList<ReadPolicyDto>> ReadPoliciesAsync()
        {
            var policies = _dbContext.Policies.Select(policy => new ReadPolicyDto
            {
                Id = policy.Id,
                PolicyNumber = policy.PolicyNumber,
                Premium = policy.Premium,
                StartDate = policy.StartDate
            });

            return policies.ToList();
        }

        /// <summary>
        /// Retrieves a policy by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns a ReadPolicyDto.</returns>
        public async Task<ReadPolicyDto?> ReadPolicyAsync(int id)
        {
            var policy = await _dbContext.Policies.SingleOrDefaultAsync(x => x.Id == id);

            if (policy == null)
            {
                return null;
            }

            var policyDto = new ReadPolicyDto()
            {
                Id = policy.Id,
                PolicyNumber = policy.PolicyNumber,
                Premium = policy.Premium,
                StartDate = policy.StartDate
            };

            return policyDto;
        }

        /// <summary>
        /// Creates a data to export.
        /// </summary>
        /// <param name="endDate"></param>
        /// <param name="startDate"></param>
        /// <returns>Returns a <ICollection<ExportDto>.</returns>
        /// <remark>Can be moved to a separate Service</remark>
        public async Task<ICollection<ExportDto>> ExportPoliciesDataByDate(DateTime startDate, DateTime endDate)
        {
            var policies = _dbContext.Policies.Include(p => p.Notes)
                            .Where(p => p.StartDate >= startDate && p.StartDate <= endDate);

            var exportDtos = new List<ExportDto>();

            await policies.ForEachAsync(p =>
            {
                exportDtos.Add(new ExportDto()
                {
                    Notes = p.Notes.Select(n => n.Text).ToList(),
                    PolicyNumber = p.PolicyNumber,
                    Premium = p.Premium,
                    StartDate = p.StartDate,
                });
            });

            return exportDtos;
        }
    }
}
