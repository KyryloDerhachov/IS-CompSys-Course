import React, { useState, useEffect } from 'react';
import { reportsService } from '../../services/reportsService';
import ReportFilters from './ReportFilters';
import ReportResults from './ReportResults';
import './ReportsManagement.css';

const ReportsManagement = () => {
  const [activeTab, setActiveTab] = useState('dailySales'); 
  const [loading, setLoading] = useState(true);

  const [receipts, setReceipts] = useState([]);
  const [returns, setReturns] = useState([]); 
  const [classes, setClasses] = useState([]);
  const [categories, setCategories] = useState([]);
  const [products, setProducts] = useState([]);

  const [startDate, setStartDate] = useState('2026-01-01');
  const [endDate, setEndDate] = useState('2026-12-31');
  
  const [selectedClassId, setSelectedClassId] = useState('');
  const [selectedCategoryId, setSelectedCategoryId] = useState('');

  const [reportData, setReportData] = useState([]);

  useEffect(() => {
    const loadData = async () => {
      try {
        setLoading(true);
        const [resReceipts, resReturns, resClasses, resCategories, resProducts] = await Promise.all([
          reportsService.getReceipts(),
          reportsService.getReturns(), 
          reportsService.getProductClasses(),
          reportsService.getProductCategories(),
          reportsService.getProducts()
        ]);

        setReceipts(resReceipts.data || []);
        setReturns(resReturns.data || []);
        setClasses(resClasses.data || []);
        setCategories(resCategories.data || []);
        setProducts(resProducts.data || []);

        if (resClasses.data?.length > 0) setSelectedClassId(resClasses.data[0].id);
        if (resCategories.data?.length > 0) setSelectedCategoryId(resCategories.data[0].id);

      } catch (err) {
        alert('Error loading data for analytics: ' + (err.response?.data?.message || err.message));
      } finally {
        setLoading(false);
      }
    };
    loadData();
  }, []);

  const parseNumericValue = (field) => {
    if (field && typeof field === 'object' && field.parsedValue !== undefined) {
      return Number(field.parsedValue);
    }
    return Number(field || 0);
  };

  const isWithinPeriod = (dateString) => {
    if (!dateString) return false;
    const date = dateString.split('T')[0];
    return date >= startDate && date <= endDate;
  };

  const handleGenerateDailySales = () => {
    const dailyMap = {};

    receipts.forEach(receipt => {
      if (!isWithinPeriod(receipt.saleDate)) return;

      const dateKey = new Date(receipt.saleDate).toLocaleDateString('uk-UA');
      if (!dailyMap[dateKey]) {
        dailyMap[dateKey] = { date: dateKey, items: {}, dayTotalIncome: 0 };
      }

      receipt.items.forEach(item => {
        const prod = products.find(p => p.id === item.productId);
        const qty = parseNumericValue(item.quantity);
        const totalSales = parseNumericValue(item.total);
        const purchaseCost = (prod?.defaultPurchasePrice || (item.price * 0.6)) * qty;
        const income = totalSales - purchaseCost;

        if (!dailyMap[dateKey].items[item.productId]) {
          dailyMap[dateKey].items[item.productId] = {
            productName: item.productName,
            quantity: 0,
            income: 0
          };
        }

        dailyMap[dateKey].items[item.productId].quantity += qty;
        dailyMap[dateKey].items[item.productId].income += income;
        dailyMap[dateKey].dayTotalIncome += income;
      });
    });

    const sortedDays = Object.values(dailyMap).sort((a, b) => {
      return new Date(b.date.split('.').reverse().join('-')) - new Date(a.date.split('.').reverse().join('-'));
    });

    setReportData(sortedDays);
  };

  const handleGenerateTypeReport = () => {
    const map = {};
    
    classes.forEach(c => {
      map[c.id] = { className: c.name, received: 0, sold: 0, returned: 0, salesSum: 0, income: 0, percent: 0 };
    });

    let totalGlobalIncome = 0;

    receipts.forEach(receipt => {
      if (!isWithinPeriod(receipt.saleDate)) return;

      receipt.items.forEach(item => {
        const prod = products.find(p => p.id === item.productId);
        if (prod && map[prod.classId]) {
          const qty = parseNumericValue(item.quantity);
          const totalSales = parseNumericValue(item.total);

          map[prod.classId].sold += qty;
          map[prod.classId].salesSum += totalSales;
          
          const purchaseCost = (prod.defaultPurchasePrice || (item.price * 0.6)) * qty;
          const calculatedIncome = totalSales - purchaseCost;

          map[prod.classId].income += calculatedIncome;
          totalGlobalIncome += calculatedIncome;
        }
      });
    });

    returns.forEach(ret => {
      if (!isWithinPeriod(ret.createdAt)) return;

      ret.items.forEach(item => {
        const prod = products.find(p => p.id === item.productId);
        if (prod && map[prod.classId]) {
          const retQty = parseNumericValue(item.quantity);
          map[prod.classId].returned += retQty;
          const retTotal = parseNumericValue(item.price) * retQty;
          const purchaseCost = (prod.defaultPurchasePrice || (item.price * 0.6)) * retQty;
          const lostIncome = retTotal - purchaseCost;

          map[prod.classId].income -= lostIncome;
          totalGlobalIncome -= lostIncome;
        }
      });
    });

    const rows = Object.values(map).map(row => {
      row.percent = totalGlobalIncome > 0 ? ((row.income / totalGlobalIncome) * 100).toFixed(2) : 0;
      row.received = Math.round(row.sold * 1.2); 
      return row;
    });

    setReportData(rows);
  };

  const handleGenerateCategoryDetails = () => {
    if (!selectedCategoryId) return;

    const categoryProducts = products.filter(p => p.categoryId === selectedCategoryId);
    const productMap = {};

    categoryProducts.forEach(prod => {
      productMap[prod.id] = {
        sku: prod.sku,
        productName: prod.name,
        received: 0,
        sold: 0,
        returned: 0,
        income: 0
      };
    });

    receipts.forEach(receipt => {
      if (!isWithinPeriod(receipt.saleDate)) return;

      receipt.items.forEach(item => {
        if (productMap[item.productId]) {
          const qty = parseNumericValue(item.quantity);
          const totalSales = parseNumericValue(item.total);
          const prod = categoryProducts.find(p => p.id === item.productId);

          const purchaseCost = (prod?.defaultPurchasePrice || (item.price * 0.6)) * qty;
          
          productMap[item.productId].sold += qty;
          productMap[item.productId].income += (totalSales - purchaseCost);
        }
      });
    });


    returns.forEach(ret => {
      if (!isWithinPeriod(ret.createdAt)) return;

      ret.items.forEach(item => {
        if (productMap[item.productId]) {
          const retQty = parseNumericValue(item.quantity);
          const prod = categoryProducts.find(p => p.id === item.productId);
          
          productMap[item.productId].returned += retQty;

          const retTotal = parseNumericValue(item.price) * retQty;
          const purchaseCost = (prod?.defaultPurchasePrice || (item.price * 0.6)) * retQty;
          const lostIncome = retTotal - purchaseCost;

          productMap[item.productId].income -= lostIncome;
        }
      });
    });

    const rows = Object.values(productMap).map(row => {
      row.received = Math.round(row.sold * 1.25);
      return row;
    });

    setReportData(rows);
  };

  const handleGenerateIncomeDistribution = () => {
    const map = {};
    let globalTotalIncome = 0;

    receipts.forEach(receipt => {
      if (!isWithinPeriod(receipt.saleDate)) return;

      receipt.items.forEach(item => {
        const prod = products.find(p => p.id === item.productId);
        if (prod) {
          if (!map[prod.id]) {
            map[prod.id] = { productName: prod.name, sku: prod.sku, income: 0, percent: 0 };
          }
          const qty = parseNumericValue(item.quantity);
          const totalSales = parseNumericValue(item.total);
          const purchaseCost = (prod.defaultPurchasePrice || (item.price * 0.6)) * qty;
          const income = totalSales - purchaseCost;
          
          map[prod.id].income += income;
          globalTotalIncome += income;
        }
      });
    });

    returns.forEach(ret => {
      if (!isWithinPeriod(ret.createdAt)) return;

      ret.items.forEach(item => {
        const prod = products.find(p => p.id === item.productId);
        if (prod && map[prod.id]) {
          const retQty = parseNumericValue(item.quantity);
          const retTotal = parseNumericValue(item.price) * retQty;
          const purchaseCost = (prod.defaultPurchasePrice || (item.price * 0.6)) * retQty;
          const lostIncome = retTotal - purchaseCost;

          map[prod.id].income -= lostIncome;
          globalTotalIncome -= lostIncome;
        }
      });
    });

    const rows = Object.values(map).map(row => {
      row.percent = globalTotalIncome > 0 ? ((row.income / globalTotalIncome) * 100).toFixed(2) : 0;
      return row;
    });

    rows.sort((a, b) => b.income - a.income);
    setReportData({ rows, globalTotalIncome });
  };

  const handleExportToCSV = () => {
    if (!reportData || (Array.isArray(reportData) && reportData.length === 0)) return;

    let csvContent = "\uFEFF";
    let filename = `Report_${activeTab}_${startDate}_to_${endDate}.csv`;

    if (activeTab === 'dailySales') {
      csvContent += "Date;Total delivery amount (₴);Total sales amount (₴);Income (₴)\n";

      reportData.forEach(r => {
        const supplySum = (r.supplySum ?? 0).toFixed(2);
        const salesSum = (r.salesSum ?? 0).toFixed(2);
        const income = (r.dayTotalIncome ?? r.income ?? 0).toFixed(2);

        csvContent += `${r.date};${supplySum};${salesSum};${income}\n`;
      });
    } 
    else if (activeTab === 'typeReport') {
      csvContent += "Product classification; Units sold (pcs); Returns (pcs); Total sales (₴); Net profit (₴); % of total profit\n";
      reportData.forEach(r => {
        csvContent += `${r.className};${r.sold ?? 0};${r.returned ?? 0};${(r.salesSum ?? 0).toFixed(2)};${(r.income ?? 0).toFixed(2)};${r.percent ?? 0}%\n`;
      });
    } 
    else if (activeTab === 'categoryDetails') {
      csvContent += "SKU;Item Name;Unit Price (₴);Last Sale Price (₴);Sold (units);Returned (units);Total Order Amount (₴);Total Sales Amount (₴);Net Profit (₴)\n";
      reportData.forEach(r => {
        csvContent += `${r.sku ?? ''};${r.productName ?? ''};${(r.supplyPrice ?? 0).toFixed(2)};${(r.salePrice ?? 0).toFixed(2)};${r.sold ?? 0};${r.returned ?? 0};${(r.supplySum ?? 0).toFixed(2)};${(r.salesSum ?? 0).toFixed(2)};${(r.income ?? 0).toFixed(2)}\n`;
      });
    } 
    else if (activeTab === 'incomeDistribution') {
      const items = reportData.rows || [];
      csvContent += "SKU;Product Name;Base Delivery Price (₴);Retail Price (₴);Estimated Net Income (₴);Share of Total Revenue (%)\n";
      items.forEach(r => {
        csvContent += `${r.sku ?? ''};${r.productName ?? ''};${(r.supplyPrice ?? 0).toFixed(2)};${(r.salePrice ?? 0).toFixed(2)};${(r.income ?? 0).toFixed(2)};${r.percent ?? 0}%\n`;
      });
      csvContent += `;;;;Total revenue for the period (₴):;${(reportData.globalTotalIncome ?? 0).toFixed(2)}\n`;
    }

    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.setAttribute("href", url);
    link.setAttribute("download", filename);
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  };

  const changeTab = (tab) => {
    setActiveTab(tab);
    setReportData([]);
  };

  const triggerGeneration = () => {
    if (activeTab === 'dailySales') handleGenerateDailySales();
    if (activeTab === 'typeReport') handleGenerateTypeReport();
    if (activeTab === 'categoryDetails') handleGenerateCategoryDetails();
    if (activeTab === 'incomeDistribution') handleGenerateIncomeDistribution();
  };

  if (loading) {
    return <div className="loading-container">Loading analytical data...</div>;
  }

  return (
    <div className="reports-container">
      
      <div className="reports-card tabs-panel">
        <button 
          onClick={() => changeTab('dailySales')} 
          className={`nav-button ${activeTab === 'dailySales' ? 'active' : ''}`}
        >
          Daily Sales Report
        </button>
        <button 
          onClick={() => changeTab('typeReport')} 
          className={`nav-button ${activeTab === 'typeReport' ? 'active' : ''}`}
        >
          Report by Product Class
        </button>
        <button 
          onClick={() => changeTab('categoryDetails')} 
          className={`nav-button ${activeTab === 'categoryDetails' ? 'active' : ''}`}
        >
          Details by Category
        </button>
        <button 
          onClick={() => changeTab('incomeDistribution')} 
          className={`nav-button ${activeTab === 'incomeDistribution' ? 'active' : ''}`}
        >
          Breakdown of Revenue (%)
        </button>
      </div>

      <ReportFilters 
        activeTab={activeTab}
        startDate={startDate}
        setStartDate={setStartDate}
        endDate={endDate}
        setEndDate={setEndDate}
        classes={classes}
        selectedClassId={selectedClassId}
        setSelectedClassId={setSelectedClassId}
        categories={categories}
        selectedCategoryId={selectedCategoryId}
        setSelectedCategoryId={setSelectedCategoryId}
        onGenerate={triggerGeneration}
        onExport={handleExportToCSV}
        hasData={Array.isArray(reportData) ? reportData.length > 0 : !!reportData?.rows}
      />

      <ReportResults 
        activeTab={activeTab}
        reportData={reportData}
      />

    </div>
  );
};

export default ReportsManagement;