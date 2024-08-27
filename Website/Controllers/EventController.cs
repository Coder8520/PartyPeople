using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using System.Diagnostics;
using Website.Models;
using Website.Persistence;

namespace Website.Controllers
{
    public class EventController : Controller
    {
        private readonly DbContext _dbContext;
        private readonly IValidator<Event> _validator;
        private readonly ILogger<EventController> _logger;

        public EventController(DbContext dbContext, IValidator<Event> validator, ILogger<EventController> logger)
        {
            _dbContext = dbContext;
            _validator = validator;
            _logger = logger;
        }

        // GET: Event
        public async Task<ActionResult> Index([FromQuery] bool showHistoricEvents = false, CancellationToken cancellationToken = default)
        {
            var events = await _dbContext.Events.GetAllAsync(showHistoricEvents, cancellationToken);
            return View(new EventListViewModel { IsShowingHistoricEvents = showHistoricEvents, Events = events });
        }

        // GET: Event/Details/5
        public async Task<ActionResult> Details(int id, CancellationToken cancellationToken)
        {
            var exists = await _dbContext.Events.ExistsAsync(id, cancellationToken);
            if (!exists)
                return NotFound();

            var @event = await _dbContext.Events.GetByIdAsync(id, cancellationToken);
            // Get Event's attending employees list
            @event.AttendingEmployees = await GetEventAttendingEmployees(id, cancellationToken);

            return View(@event);
        }

        // GET: Event/Create
        public ActionResult Create()
        {
            var allEmployees = _dbContext.Employees.GetAllAsync().Result.ToList();

            Event @event = new Event
            {
                Id = -1,
                Description = "",
                StartDateTime = DateTime.Now,
                EndDateTime = DateTime.Now.AddHours(1),
                AttendingEmployees = allEmployees
            };

            return View(@event);
        }

        // POST: Event/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Event @event, CancellationToken cancellationToken)
        {
            //IEnumerable<Employee> viewBagEmployees = ViewBag.Employees;
            @event.AttendingEmployees = @event.AttendingEmployees.Where(e => e.IsAttendingEvent == true).ToList();

            var validationResult = await _validator.ValidateAsync(@event, cancellationToken);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return View(@event);
            }

            var createdEvent = await _dbContext.Events.CreateAsync(@event, cancellationToken);

            // update EmployeeEvents table with attending employees
            if (@event.AttendingEmployees.Count() > 0)
            {
                foreach (var employee in @event.AttendingEmployees)
                {
                    // INSERT to EmployeeEvents
                    await _dbContext.EmployeeEvents.InsertAsync(createdEvent.Id, employee.Id);
                }
            }

            return RedirectToAction(nameof(Details), new { id = createdEvent.Id });
        }

        // GET: Event/Edit/5
        public async Task<ActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var exists = await _dbContext.Events.ExistsAsync(id, cancellationToken);
            if (!exists)
                return NotFound();

            var @event = await _dbContext.Events.GetByIdAsync(id, cancellationToken);

            // get all employees
            var allEmployees = await _dbContext.Employees.GetAllAsync(cancellationToken);
            var attendingEmployees = await GetEventAttendingEmployees(id, cancellationToken);
            // set is attending bool for checkbox
            foreach (var employee in allEmployees)
            {
                if (attendingEmployees.Any(e => e.Id == employee.Id))
                {
                    employee.IsAttendingEvent = true;
                }
                else 
                { 
                    employee.IsAttendingEvent = false;
                }
            }

            EventEditModel eventEditModel = new EventEditModel
            {
                Event = @event,
                Employees = allEmployees.OrderByDescending(e => e.IsAttendingEvent).ToList()
            };

            return View(eventEditModel);
        }

        // POST: Event/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, EventEditModel eventEditmodel,  CancellationToken cancellationToken)
        {
            Event currentEvent = eventEditmodel.Event; 
            currentEvent.AttendingEmployees = eventEditmodel.Employees.Where(e => e.IsAttendingEvent == true).ToList();
            
            var validationResult = await _validator.ValidateAsync(currentEvent, cancellationToken);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return View(eventEditmodel);
            }

            var updatedEvent = await _dbContext.Events.UpdateAsync(currentEvent, cancellationToken);

            // Update attending employees
            var allEmployees = eventEditmodel.Employees;

            foreach (var employee in allEmployees)
            {
                // TO DO could move to helper this insert exists delete block
                // check employee exists in EmployeeEvents table
                bool exists = await _dbContext.EmployeeEvents.ExistsAsync(updatedEvent.Id, employee.Id, cancellationToken);

                if (employee.IsAttendingEvent && !exists)
                {
                    // INSERT to EmployeeEvents
                    await _dbContext.EmployeeEvents.InsertAsync(updatedEvent.Id, employee.Id);
                }
                else if (!employee.IsAttendingEvent && exists)
                {
                    await _dbContext.EmployeeEvents.DeleteAsync(updatedEvent.Id, employee.Id);
                }
            }

            return RedirectToAction(nameof(Details), new { id = updatedEvent.Id });
        }

        // GET: Event/Delete/5
        public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var exists = await _dbContext.Events.ExistsAsync(id, cancellationToken);
            if (!exists)
                return NotFound();

            // also deletes from EmployeeEvents table
            await _dbContext.Events.DeleteAsync(id, cancellationToken);

            return RedirectToAction(nameof(Index));
        }

        // Helpers
        private async Task<IList<Employee>> GetEventAttendingEmployees(int id, CancellationToken cancellationToken)
        {
            var attendingEmployees = await _dbContext.Employees.GetAttendingEmployeesByEventIdAsync(id, cancellationToken);
            if (attendingEmployees != null && attendingEmployees.Any())
            {
                return attendingEmployees.ToList();
            }

            return new List<Employee>();
        }
    }
}
