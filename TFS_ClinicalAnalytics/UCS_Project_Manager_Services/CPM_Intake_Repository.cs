//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using UCS_Project_Manager;
//using System.Data.Entity;


//namespace UCS_Project_Manager_Services
//{



//    public interface ICPM_Intake_Repository
//    {
//        Task<List<CPM_Intake_Model>> GetCPM_IntakeAsync();
//        Task<CPM_Intake_Model> GetProjectIntakeSampl1eAsync(Int64 id);
//        Task<CPM_Intake_Model> AddCPM_IntakeAsync(CPM_Intake_Model CPM_Intake);
//        Task<CPM_Intake_Model> UpdateCPM_IntakeAsync(CPM_Intake_Model CPM_Intake);
//        Task DeleteCPM_Intake_ModelAsync(Int64 id);
//    }

 

//    public class CPM_Intake_Repository : ICPM_Intake_Repository
//    {

//        private _EFMainContext _context = new _EFMainContext();
        

//        public async Task<List<CPM_Intake_Model>> GetCPM_IntakeAsync()
//        {


//            //var a = await _context.CPM_Intake.ToListAsync(); 

//            return await _context.CPM_Intake.ToListAsync();
//        }

//        public Task<CPM_Intake_Model> GetProjectIntakeSampl1eAsync(Int64 id)
//        {
//            return _context.CPM_Intake.FirstOrDefaultAsync(c => c.IntakeId == id);
//        }

//        public async Task<CPM_Intake_Model> AddCPM_IntakeAsync(CPM_Intake_Model CPM_Intake_Model)
//        {
//            //ADDED 6222021 IN HOPES TO SPEED UP :(
//            _context.Configuration.AutoDetectChangesEnabled = false;

//            _context.CPM_Intake.Add(CPM_Intake_Model);

//            //ALSO ADDED 6222021 IN HOPES TO SPEED UP 
//            //BUT THIS FIXED KEY BUG :)
//            _context.ChangeTracker.DetectChanges();

//            await _context.SaveChangesAsync();
//            return CPM_Intake_Model;
//        }

//        public async Task<CPM_Intake_Model> UpdateCPM_IntakeAsync(CPM_Intake_Model CPM_Intake_Model)
//        {

//            //if (!_context.CPM_Intake.Local.Any(c => c.Id == CPM_Intake_Model.Id))
//            if (!(await _context.CPM_Intake.AnyAsync(c => c.IntakeId == CPM_Intake_Model.IntakeId)))
//            {
//                _context.CPM_Intake.Attach(CPM_Intake_Model);
//            }

//            _context.Entry(CPM_Intake_Model).State = EntityState.Modified;
//            await _context.SaveChangesAsync();
//            return CPM_Intake_Model;

//        }

//        public async Task DeleteCPM_Intake_ModelAsync(Int64 id)
//        {
//            //var CPM_Intake_Model = _context.CPM_Intake.FirstOrDefault(c => c.Id == CPM_Intake_ModelId);
//            var CPM_Intake_Model = await _context.CPM_Intake.FirstOrDefaultAsync(c => c.IntakeId == id);

//            if (CPM_Intake_Model != null)
//            {
//                _context.CPM_Intake.Remove(CPM_Intake_Model);
//            }

//            await _context.SaveChangesAsync();
//        }
//    }

   
//}
