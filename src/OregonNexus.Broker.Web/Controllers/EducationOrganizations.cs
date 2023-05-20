
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OregonNexus.Broker.SharedKernel;
using OregonNexus.Broker.Domain;
using OregonNexus.Broker.Web.Models;

namespace OregonNexus.Broker.Web.Controllers;

public class EducationOrganizationsController : Controller
{
    private IRepository<EducationOrganization> _repo { get; set; }
    
    public EducationOrganizationsController(IRepository<EducationOrganization> repo)
    {
        _repo = repo;
    }

    public async Task<IActionResult> Index()
    {
        var data = await _repo.ListAsync();
        data = data.OrderBy(x => x.Name).ToList();

        return View(data);
    }

    public IActionResult Add()
    {
        return View();
    }

    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> Create(EducationOrganizationViewModel data)
    {
        if (!ModelState.IsValid) { TempData["Error"] = "Organization not created."; return View("Add"); }

        var organization = new EducationOrganization()
        {
            Id = Guid.NewGuid(),
            Name = data.Name,
            Number = data.Number,
            EducationOrganizationType = data.EducationOrganizationType
        };

        await _repo.AddAsync(organization);

        TempData["Success"] = $"Created organization {organization.Name} ({organization.Id}).";

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(Guid Id)
    {
        var organization = await _repo.GetByIdAsync(Id);

        var organizationViewModel = new EducationOrganizationViewModel();

        if (organization is not null)
        {
            organizationViewModel = new EducationOrganizationViewModel()
            {
                EducationOrganizationId = organization.Id,
                Name = organization.Name!,
                EducationOrganizationType = organization.EducationOrganizationType,
                Number = organization.Number!
            };
        }

        return View(organizationViewModel);
    }

    [ValidateAntiForgeryToken]
    [HttpPatch]
    public async Task<IActionResult> Update(EducationOrganizationViewModel data)
    {
        if (!data.EducationOrganizationId.HasValue) { throw new ArgumentNullException("EducationOrganizationId required."); }
        
        // Get existing organization
        var organization = await _repo.GetByIdAsync(data.EducationOrganizationId.Value);

        if (organization is null) { throw new ArgumentException("Not a valid organization."); }

        if (!ModelState.IsValid) { TempData["Error"] = "Organization not updated."; return View("Edit"); }

        // Prepare organization object
        organization.Name = data.Name;
        organization.Number = data.Number;
        organization.EducationOrganizationType = data.EducationOrganizationType;

        await _repo.UpdateAsync(organization);

        TempData["Success"] = $"Updated organization {organization.Name} ({organization.Id}).";

        return RedirectToAction("Edit", new { Id = organization.Id });
    }

    [ValidateAntiForgeryToken]
    [HttpDelete]
    public async Task<IActionResult> Delete(Guid? id)
    {
        var organization = await _repo.GetByIdAsync(id.Value);

        if (organization is null) { throw new ArgumentException("Not a valid organization."); }

        await _repo.DeleteAsync(organization);

        TempData["Success"] = $"Deleted organization {organization.Name} ({organization.Id}).";

        return RedirectToAction("Index");
    }

}