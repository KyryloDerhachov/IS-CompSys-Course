import React from 'react';

const ReportResults = ({ activeTab, reportData }) => {
  const isDataEmpty = !reportData || (Array.isArray(reportData) && reportData.length === 0);

  return (
    <div className="reports-card">
      <h4 style={{ marginTop: 0 }}>Search results</h4>
      

      {activeTab === 'dailySales' && Array.isArray(reportData) && (
        !isDataEmpty ? (
          <div className="daily-sales-wrapper">
            {reportData.map((dayBlock, index) => (
              <div key={index} className="day-report-block" style={{ marginBottom: '25px', borderBottom: '1px solid #eee', paddingBottom: '15px' }}>
                <h5 style={{ backgroundColor: '#34659491', padding: '8px 12px', margin: '10px 0' }}>
                  Date: <strong>{dayBlock.date}</strong> | Total profit for the day: <span className="text-success"><strong>{dayBlock.dayTotalIncome.toFixed(2)} ₴</strong></span>
                </h5>
                <table className="reports-table">
                  <thead>
                    <tr>
                      <th>Product Name</th>
                      <th>Number sold</th>
                      <th>Profit by item</th>
                    </tr>
                  </thead>
                  <tbody>
                    {Object.values(dayBlock.items).map((item, i) => (
                      <tr key={i}>
                        <td>{item.productName}</td>
                        <td>{item.quantity} pcx</td>
                        <td><strong className="text-success">{item.income.toFixed(2)} ₴</strong></td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            ))}
          </div>
        ) : <p className="alert-message">No sales matching these criteria were found.</p>
      )}

      {activeTab === 'typeReport' && Array.isArray(reportData) && (
        !isDataEmpty ? (
          <table className="reports-table">
            <thead>
              <tr>
                <th>Product Type (Class)</th>
                <th>Received</th>
                <th>Sold</th>
                <th>Returned</th>
                <th>Sales amount</th>
                <th>Income</th>
                <th>% of total revenue</th>
              </tr>
            </thead>
            <tbody>
              {reportData.map((row, i) => (
                <tr key={i}>
                  <td><strong>{row.className}</strong></td>
                  <td>{row.received} pcx</td>
                  <td>{row.sold} pcx</td>
                  <td>{row.returned} pcx</td>
                  <td>{row.salesSum.toFixed(2)} ₴</td>
                  <td className="text-success">{row.income.toFixed(2)} ₴</td>
                  <td className="text-primary">{row.percent}%</td>
                </tr>
              ))}
            </tbody>
          </table>
        ) : <p className="text-primary" style={{ textAlign: 'center', padding: '20px' }}>Select the period and click the button above to calculate.</p>
      )}

      {activeTab === 'categoryDetails' && Array.isArray(reportData) && (
        !isDataEmpty ? (
          <table className="reports-table">
            <thead>
              <tr>
                <th>SKU</th>
                <th>Name of the specific product</th>
                <th>Number of units delivered</th>
                <th>Number of units sold</th>
                <th>Number of returned items</th>
                <th>Profit</th>
              </tr>
            </thead>
            <tbody>
              {reportData.map((row, i) => (
                <tr key={i}>
                  <td><code>{row.sku}</code></td>
                  <td><strong>{row.productName}</strong></td>
                  <td>{row.received} pcx</td>
                  <td>{row.sold} pcx</td>
                  <td>{row.returned} pcx</td>
                  <td className="text-success">{row.income.toFixed(2)} ₴</td>
                </tr>
              ))}
            </tbody>
          </table>
        ) : <p className="text-primary" style={{ textAlign: 'center', padding: '20px' }}>No data available, or there were no transactions in the selected category.</p>
      )}

      {activeTab === 'incomeDistribution' && reportData?.rows && (
        <div>
          <div className="info-banner">
            The company's revenue for the period: <strong>{reportData.globalTotalIncome.toFixed(2)} ₴</strong>
          </div>
          <table className="reports-table">
            <thead>
              <tr>
                <th>SKU</th>
                <th>Name of the specific product</th>
                <th>Income</th>
                <th>Percentage of total profit</th>
              </tr>
            </thead>
            <tbody>
              {reportData.rows.map((row, i) => (
                <tr key={i}>
                  <td><code>{row.sku}</code></td>
                  <td>{row.productName}</td>
                  <td className="text-success">{row.income.toFixed(2)} ₴</td>
                  <td>
                    <div className="progress-bar-container">
                      <div 
                        className="progress-bar-track" 
                        style={{ width: `${Math.max(row.percent, 2)}%` }}
                      ></div>
                      <span>{row.percent}%</span>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

    </div>
  );
};

export default ReportResults;