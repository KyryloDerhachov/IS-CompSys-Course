import React, { useState, useEffect } from 'react';
import { stockService } from '../../services/stockService';
import { catalogService } from '../../services/catalogService';

import BatchesTab from './BatchesTab';
import ExpiringTab from './ExpiringTab';
import NewSupplyTab from './NewSupplyTab';
import ReduceStockModal from './ReduceStockModal';

import './StockManagement.css';

const StockManagement = () => {
  const [activeTab, setActiveTab] = useState('batches');
  const [batches, setBatches] = useState([]);
  const [expiringBatches, setExpiringBatches] = useState([]);
  const [products, setProducts] = useState([]); 
  const [loading, setLoading] = useState(true);
  const [daysThreshold, setDaysThreshold] = useState(7);

  const [reduceModal, setReduceModal] = useState({ open: false, batch: null, quantity: '', version: 0 });

  const [supplyForm, setSupplyForm] = useState({
    supplierName: '',
    supplyDate: new Date().toISOString().split('T')[0],
    items: []
  });

  const [newItem, setNewItem] = useState({ productId: '', quantity: '', purchasePrice: '', shelfLifeDays: '30' });

  const loadStockData = async () => {
    try {
      setLoading(true);
      const [resBatches, resExpiring, resProducts] = await Promise.all([
        stockService.getBatches(),
        stockService.getExpiringStocks(daysThreshold),
        catalogService.getProducts(false)
      ]);
      setBatches(resBatches.data || []);
      setExpiringBatches(resExpiring.data || []);
      setProducts(resProducts.data || []);
    } catch (err) {
      alert('Error loading inventory data: ' + (err.response?.data?.message || err.message));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadStockData();
  }, [daysThreshold]);

  const handleReduceStock = async (e) => {
    e.preventDefault();
    if (!reduceModal.batch) return;

    try {
      await stockService.reduceStock(reduceModal.batch.id, reduceModal.quantity, reduceModal.version);
      setReduceModal({ open: false, batch: null, quantity: '', version: 0 });
      loadStockData();
    } catch (err) {
      alert('Transaction error: ' + (err.response?.data?.message || 'Please refresh the page; the information is out of date.'));
    }
  };

  const addItemToSupplyList = () => {
    if (!newItem.productId || !newItem.quantity || !newItem.purchasePrice) {
      alert('Please fill in all the fields for this product listing');
      return;
    }
    const selectedProd = products.find(p => p.id === newItem.productId);
    
    setSupplyForm({
      ...supplyForm,
      items: [...supplyForm.items, {
        productId: newItem.productId,
        productName: selectedProd?.name,
        quantity: parseFloat(newItem.quantity),
        purchasePrice: parseFloat(newItem.purchasePrice),
        shelfLifeDays: parseInt(newItem.shelfLifeDays, 10)
      }]
    });
    setNewItem({ productId: '', quantity: '', purchasePrice: '', shelfLifeDays: '30' });
  };

  const handleCSVUpload = (e) => {
    const file = e.target.files[0];
    if (!file) return;

    const reader = new FileReader();
    reader.onload = (event) => {
      const text = event.target.result;
      const lines = text.split(/\r?\n/).map(line => line.trim()).filter(line => line.length > 0);
      
      if (lines.length <= 1) {
        alert("The file is empty or contains only a header");
        return;
      }

      const importedItems = [];
      const skippedRows = [];

      for (let i = 1; i < lines.length; i++) {
        const delimiter = lines[i].includes(';') ? ';' : ',';
        const columns = lines[i].split(delimiter).map(col => col.trim());

        if (columns.length < 4) {
          skippedRows.push(`Line ${i + 1}: Not enough speakers.`);
          continue;
        }

        const fileSku = columns[0];
        const fileQuantity = parseFloat(columns[1]);
        const filePrice = parseFloat(columns[2]);
        const fileShelfLife = parseInt(columns[3], 10);

        const targetProduct = products.find(p => p.sku?.toLowerCase() === fileSku?.toLowerCase());

        if (!targetProduct) {
          skippedRows.push(`Line ${i + 1}: The product with SKU "${fileSku}" was not found in the database.`);
          continue;
        }

        if (isNaN(fileQuantity) || fileQuantity <= 0 || isNaN(filePrice) || filePrice < 0) {
          skippedRows.push(`Line ${i + 1}: Invalid numerical data for quantity or price.`);
          continue;
        }

        importedItems.push({
          productId: targetProduct.id,
          productName: targetProduct.name,
          quantity: fileQuantity,
          purchasePrice: filePrice,
          shelfLifeDays: isNaN(fileShelfLife) ? 30 : fileShelfLife
        });
      }

      setSupplyForm(prev => ({
        ...prev,
        items: [...prev.items, ...importedItems]
      }));

      if (skippedRows.length > 0) {
        alert(`Items imported: ${importedItems.length}.\n\nSkip lines:\n${skippedRows.join('\n')}`);
      } else {
        alert(`All items from the file (${importedItems.length} pcs.) successfully added to the packing slip`);
      }
      
      e.target.value = null;
    };

    reader.readAsText(file, "UTF-8");
  };

  const removeSupplyItem = (index) => {
    const updated = [...supplyForm.items];
    updated.splice(index, 1);
    setSupplyForm({ ...supplyForm, items: updated });
  };

  const handleSubmitSupply = async (e) => {
    e.preventDefault();
    if (supplyForm.items.length === 0) {
      alert('You cannot create a shipment without items');
      return;
    }

    try {
      const resCreate = await stockService.createSupply({
        supplierName: supplyForm.supplierName,
        supplyDate: new Date(supplyForm.supplyDate).toISOString(),
        items: supplyForm.items.map(({ productId, quantity, purchasePrice, shelfLifeDays }) => ({
          productId, quantity, purchasePrice, shelfLifeDays
        }))
      });

      const supplyId = resCreate.data;
      await stockService.postSupply(supplyId, 0);

      alert('The shipment has been successfully registered and delivered to the warehouse');
      setSupplyForm({ supplierName: '', supplyDate: new Date().toISOString().split('T')[0], items: [] });
      setActiveTab('batches');
      loadStockData();
    } catch (err) {
      alert('Error during delivery: ' + (err.response?.data?.message || err.message));
    }
  };

  return (
    <div className="stock-container">
      <div className="stock-header">
        <h2>Inventory Management: Shipments and Batches</h2>
      </div>

      <div className="tabs-container">
        <button 
          onClick={() => setActiveTab('batches')} 
          className={`tab-button ${activeTab === 'batches' ? 'active-batches' : ''}`}
        >
          Current balances (Batches)
        </button>
        <button 
          onClick={() => setActiveTab('expiring')} 
          className={`tab-button ${activeTab === 'expiring' ? 'active-expiring' : ''}`}
        >
          Expiration Date Monitoring
        </button>
        <button 
          onClick={() => setActiveTab('new-supply')} 
          className={`tab-button ${activeTab === 'new-supply' ? 'active-supply' : ''}`}
        >
          Receipt (New Delivery)
        </button>
      </div>

      {loading ? (
        <p>Updating data from the inventory database...</p>
      ) : (
        <div className="stock-content">
          {activeTab === 'batches' && (
            <BatchesTab 
              batches={batches} 
              onReduceClick={(b) => setReduceModal({ open: true, batch: b, quantity: '', version: b.version })} 
            />
          )}

          {activeTab === 'expiring' && (
            <ExpiringTab
              expiringBatches={expiringBatches}
              daysThreshold={daysThreshold}
              onThresholdChange={setDaysThreshold}
              onReduceClick={(b) =>
                setReduceModal({
                  open: true,
                  batch: b,
                  quantity: '',
                  version: b.version
                })
              }
            />
          )}

          {activeTab === 'new-supply' && (
            <NewSupplyTab 
              supplyForm={supplyForm}
              setSupplyForm={setSupplyForm}
              newItem={newItem}
              setNewItem={setNewItem}
              products={products}
              onCSVUpload={handleCSVUpload}
              onAddItem={addItemToSupplyList}
              onRemoveItem={removeSupplyItem}
              onSubmit={handleSubmitSupply}
            />
          )}
        </div>
      )}
      
      {reduceModal.open && (
        <ReduceStockModal 
          modalData={reduceModal}
          onQuantityChange={(val) => setReduceModal({ ...reduceModal, quantity: val })}
          onClose={() => setReduceModal({ open: false, batch: null, quantity: '', version: 0 })}
          onSubmit={handleReduceStock}
        />
      )}
    </div>
  );
};

export default StockManagement;