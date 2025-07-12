using FlowingDefault.Core.Models;
using FlowingDefault.Core.Services;
using FlowingDefault.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FlowingDefault.Core.Dtos;

namespace FlowingDefault.Api.Controllers
{
    [Route("[controller]")]
    public class LabelController : ControllerBase
    {
        private readonly ILogger<LabelController> _logger;
        private readonly LabelService _labelService;

        public LabelController(ILogger<LabelController> logger, LabelService labelService)
        {
            _logger = logger;
            _labelService = labelService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Label>>> GetAll()
        {
            try
            {
                var labels = await _labelService.GetAll();
                return Ok(labels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all labels");
                return StatusCode(500, "An error occurred while retrieving labels");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Label>> GetById(int id)
        {
            try
            {
                var label = await _labelService.GetById(id);
                if (label == null)
                    return NotFound($"Label with ID {id} not found");
                return Ok(label);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving label with ID {LabelId}", id);
                return StatusCode(500, "An error occurred while retrieving the label");
            }
        }

        [HttpPost]
        public async Task<ActionResult<LabelDto>> Create([FromBody] LabelDto labelDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                await _labelService.Save(labelDto);
                return Ok(labelDto);
            }
            catch (FlowingDefaultException ex)
            {
                _logger.LogWarning(ex, "Business rule violation while creating label");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating label");
                return StatusCode(500, "An error occurred while creating the label");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<LabelDto>> Update(int id, [FromBody] LabelDto labelDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var existingLabel = await _labelService.GetById(id);
                if (existingLabel == null)
                    return NotFound($"Label with ID {id} not found");
                labelDto.Id = id;
                await _labelService.Save(labelDto);
                return Ok(labelDto);
            }
            catch (FlowingDefaultException ex)
            {
                _logger.LogWarning(ex, "Business rule violation while updating label {LabelId}", id);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating label {LabelId}", id);
                return StatusCode(500, "An error occurred while updating the label");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _labelService.Delete(id);
                if (!deleted)
                    return NotFound($"Label with ID {id} not found");
                return NoContent();
            }
            catch (FlowingDefaultException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting label {LabelId}", id);
                return StatusCode(500, "An error occurred while deleting the label");
            }
        }
    }
} 