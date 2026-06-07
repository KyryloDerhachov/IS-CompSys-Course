import React from 'react';

const ReportFilters = ({
  activeTab,
  startDate,
  setStartDate,
  endDate,
  setEndDate,
  classes,
  selectedClassId,
  setSelectedClassId,
  categories,
  selectedCategoryId,
  setSelectedCategoryId,
  onGenerate,
  onExport,
  hasData
}) => {
  return (
    <div className="reports-card">
      <h3 style={{ marginTop: 0, marginBottom: '15px' }}>Report Generation Settings</h3>
      <div className="filters-wrapper">
        
        <div className="filter-box">
          <label>Start of the period:</label>
          <input 
            type="date" 
            value={startDate} 
            onChange={e => setStartDate(e.target.value)} 
            className="filter-input" 
          />
        </div>

        <div className="filter-box">
          <label>End of period:</label>
          <input 
            type="date" 
            value={endDate} 
            onChange={e => setEndDate(e.target.value)} 
            className="filter-input" 
          />
        </div>

        {activeTab === 'categoryDetails' && (
          <div className="filter-box">
            <label>Select a Category:</label>
            <select 
              value={selectedCategoryId} 
              onChange={e => setSelectedCategoryId(e.target.value)} 
              className="filter-input"
            >
              {categories.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
            </select>
          </div>
        )}

        <div className="actions-box">
          <button onClick={onGenerate} className="btn btn-success">
            {activeTab === 'dailySales' && 'Analyze sales by day'}
            {activeTab === 'typeReport' && 'Generate a report by class'}
            {activeTab === 'categoryDetails' && 'Sort by category'}
            {activeTab === 'incomeDistribution' && 'Analyze the distribution'}
          </button>
          
          {hasData && (
            <button onClick={onExport} className="btn btn-info">
               Export to .CSV
            </button>
          )}
        </div>

      </div>
    </div>
  );
};

export default ReportFilters;