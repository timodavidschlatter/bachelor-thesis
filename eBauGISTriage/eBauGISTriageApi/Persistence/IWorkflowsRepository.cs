using RulesEngine.Models;
using System.Collections.Generic;

namespace eBauGISTriageApi.Persistence
{
    /// <summary>
    /// This file contains the interface to define WorkflowsRepositories.
    /// It is using the repository pattern to allow to change the workflow storage.
    /// </summary>
    public interface IWorkflowsRepository
    {
        /// <summary>
        /// Returns all workflows from storage. 
        /// </summary>
        /// <returns>A list containing the workflows.</returns>
        List<Workflow> GetWorkflows();
    }
}
