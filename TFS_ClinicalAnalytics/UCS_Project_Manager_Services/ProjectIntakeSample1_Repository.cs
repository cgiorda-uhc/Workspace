//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using UCS_Project_Manager;
//using System.Data.Entity;


//namespace UCS_Project_Manager_Services
//{



//    public interface IProjectIntakeSample1_Repository
//    {
//        Task<List<ProjectIntakeSample1_Model>> GetProjectIntakeSample1Async();
//        Task<ProjectIntakeSample1_Model> GetProjectIntakeSampl1eAsync(Guid id);
//        Task<ProjectIntakeSample1_Model> AddProjectIntakeSample1Async(ProjectIntakeSample1_Model projectIntakeSample1);
//        Task<ProjectIntakeSample1_Model> UpdateProjectIntakeSample1Async(ProjectIntakeSample1_Model projectIntakeSample1);
//        Task DeleteProjectIntakeSample1_ModelAsync(Guid id);
//    }

 

//    public class ProjectIntakeSample1_Repository : IProjectIntakeSample1_Repository
//    {

//        private _EFMainContext _context = new _EFMainContext();
        

//        public async Task<List<ProjectIntakeSample1_Model>> GetProjectIntakeSample1Async()
//        {


//            //var a = await _context.projectIntakeSample1.ToListAsync(); 

//            return await _context.projectIntakeSample1.ToListAsync();
//        }

//        public Task<ProjectIntakeSample1_Model> GetProjectIntakeSampl1eAsync(Guid id)
//        {
//            return _context.projectIntakeSample1.FirstOrDefaultAsync(c => c.guidIntakeId == id);
//        }

//        public async Task<ProjectIntakeSample1_Model> AddProjectIntakeSample1Async(ProjectIntakeSample1_Model ProjectIntakeSample1_Model)
//        {
//            //ADDED 6222021 IN HOPES TO SPEED UP :(
//            _context.Configuration.AutoDetectChangesEnabled = false;

//            _context.projectIntakeSample1.Add(ProjectIntakeSample1_Model);

//            //ALSO ADDED 6222021 IN HOPES TO SPEED UP 
//            //BUT THIS FIXED KEY BUG :)
//            _context.ChangeTracker.DetectChanges();

//            await _context.SaveChangesAsync();
//            return ProjectIntakeSample1_Model;
//        }

//        public async Task<ProjectIntakeSample1_Model> UpdateProjectIntakeSample1Async(ProjectIntakeSample1_Model ProjectIntakeSample1_Model)
//        {

//            //if (!_context.projectIntakeSample1.Local.Any(c => c.Id == ProjectIntakeSample1_Model.Id))
//            if (!(await _context.projectIntakeSample1.AnyAsync(c => c.guidIntakeId == ProjectIntakeSample1_Model.guidIntakeId)))
//            {
//                _context.projectIntakeSample1.Attach(ProjectIntakeSample1_Model);
//            }

//            _context.Entry(ProjectIntakeSample1_Model).State = EntityState.Modified;
//            await _context.SaveChangesAsync();
//            return ProjectIntakeSample1_Model;

//        }

//        public async Task DeleteProjectIntakeSample1_ModelAsync(Guid id)
//        {
//            //var ProjectIntakeSample1_Model = _context.projectIntakeSample1.FirstOrDefault(c => c.Id == ProjectIntakeSample1_ModelId);
//            var ProjectIntakeSample1_Model = await _context.projectIntakeSample1.FirstOrDefaultAsync(c => c.guidIntakeId == id);

//            if (ProjectIntakeSample1_Model != null)
//            {
//                _context.projectIntakeSample1.Remove(ProjectIntakeSample1_Model);
//            }

//            await _context.SaveChangesAsync();
//        }
//    }

   
//}
