import React, { useState, useEffect } from 'react';
import { salesService } from '../../services/salesService';
import { catalogService } from '../../services/catalogService';
import { stockService } from '../../services/stockService';
import './SalesManagement.css';

const SalesManagement = () => {
  const [activeTab, setActiveTab] = useState('pos');
  const [products, setProducts] = useState([]);
  const [history, setHistory] = useState([]);
  const [loading, setLoading] = useState(true);
  const [cart, setCart] = useState([]);
  const [skuSearch, setSkuSearch] = useState('');
  const [suggestions, setSuggestions] = useState([]);
  const [returnModal, setReturnModal] = useState({ open: false, receipt: null, items: [] });

  const [searchReceiptId, setSearchReceiptId] = useState('');
  const [selectedReceipt, setSelectedReceipt] = useState(null);

  const loadSalesData = async () => {
    try {
      setLoading(true);
      const [resProducts, resHistory] = await Promise.all([
        catalogService.getProducts(false),
        salesService.getHistory()
      ]);
      setProducts(resProducts.data || []);
      setHistory(resHistory.data || []);
    } catch (err) {
      alert('Data loading error: ' + (err.response?.data?.message || err.message));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadSalesData();
  }, []);

  const handleSearchChange = (e) => {
    const value = e.target.value;
    setSkuSearch(value);
    const cleanValue = value.trim().toLowerCase();

    if (!cleanValue) {
      setSuggestions([]);
      return;
    }

    const filtered = products.filter(p =>
      p.isActive && (
        p.sku.toLowerCase().includes(cleanValue) ||
        (p.barcode && p.barcode.includes(cleanValue)) ||
        p.name.toLowerCase().includes(cleanValue)
      )
    );
    setSuggestions(filtered.slice(0, 8));
  };

  const addProductToCart = async (product) => {
    try {
      const resBatches = await stockService.getBatches(product.id);
      const batches = resBatches.data || [];
      const totalStock = batches
        .filter(b => !b.isExpired)
        .reduce((sum, b) => sum + (b.remainingQuantity || 0), 0);

      const existingIndex = cart.findIndex(item => item.productId === product.id);

      if (existingIndex > -1) {
        const updatedCart = [...cart];
        updatedCart[existingIndex].quantity += 1;
        updatedCart[existingIndex].stock = totalStock;
        setCart(updatedCart);
      } else {
        setCart([...cart, {
          productId: product.id,
          sku: product.sku,
          name: product.name,
          quantity: 1,
          defaultSalePrice: product.defaultSalePrice || 0,
          stock: totalStock
        }]);
      }
    } catch (err) {
      alert('Error retrieving balances: ' + (err.response?.data?.message || err.message));
    }

    setSkuSearch('');
    setSuggestions([]);
  };

  const handleAddBySku = (e) => {
    e.preventDefault();
    const cleanSku = skuSearch.trim().toLowerCase();
    if (!cleanSku) return;

    const product = products.find(p =>
      p.isActive && (
        p.sku.toLowerCase() === cleanSku ||
        (p.barcode && p.barcode === cleanSku) ||
        p.name.toLowerCase() === cleanSku
      )
    );

    if (!product) {
      alert('Product not found');
      return;
    }

    addProductToCart(product);
  };

  const updateQuantity = (index, value) => {
    const updated = [...cart];
    const qty = parseInt(value, 10);
    updated[index].quantity = qty > 0 ? qty : 1;
    setCart(updated);
  };

  const removeFromCart = (index) => {
    const updated = [...cart];
    updated.splice(index, 1);
    setCart(updated);
  };

  const totalSum = cart.reduce((sum, item) => sum + (item.quantity * item.defaultSalePrice), 0);
  const hasStockError = cart.some(item => item.quantity > item.stock);

  const handleCheckout = async () => {
    if (cart.length === 0) {
      alert('The receipt is blank');
      return;
    }
    if (hasStockError) {
      alert('We are out of stock on some items');
      return;
    }

    try {
      const payload = {
        items: cart.map(item => ({
          productId: item.productId,
          quantity: item.quantity,
          price: item.defaultSalePrice
        }))
      };

      await salesService.createReceipt(payload);
      alert('The transaction was successful');
      
      setCart([]);
      loadSalesData();
    } catch (err) {
      alert('Receipt error: ' + (err.response?.data?.message || err.message));
    }
  };

  const filteredHistory = history.filter(receipt =>
    receipt.id.toLowerCase().includes(searchReceiptId.toLowerCase()) ||
    (receipt.receiptNumber && receipt.receiptNumber.toLowerCase().includes(searchReceiptId.toLowerCase()))
  );

  const openReceiptDetails = (receipt) => {
    setSelectedReceipt(receipt);
  };

  const openReturnModal = (receipt) => {
    const items = receipt.items.map(item => {
      const purchasedQty = typeof item.quantity === 'object' 
        ? (parseInt(item.quantity?.parsedValue, 10) || 0)
        : (parseInt(item.quantity, 10) || 0);

      return {
        productId: item.productId,
        name: item.productName || 'Cargo',
        maxQuantity: purchasedQty,
        quantity: 0
      };
    });
    setReturnModal({ open: true, receipt, items });
  };

  const handleReturnQtyChange = (index, val) => {
    const updated = [...returnModal.items];
    const qty = parseInt(val, 10) || 0;
    updated[index].quantity = Math.max(0, Math.min(qty, updated[index].maxQuantity));
    setReturnModal({ ...returnModal, items: updated });
  };

  const submitReturn = async (e) => {
    e.preventDefault();
    const itemsToReturn = returnModal.items.filter(i => i.quantity > 0);

    if (itemsToReturn.length === 0) {
      alert('Please specify the quantity to be returned');
      return;
    }

    try {
      await salesService.processReturn(returnModal.receipt.id, {
        items: itemsToReturn.map(i => ({ productId: i.productId, quantity: i.quantity }))
      });
      alert('The return has been successfully processed');
      setReturnModal({ open: false, receipt: null, items: [] });
      loadSalesData();
    } catch (err) {
      alert('Error in processing the return: ' + (err.response?.data?.message || err.message));
    }
  };

  return (
    <div className="sales-container">
      <div className="sales-header">
        <div className="tabs">
          <button
            className={`tab-button ${activeTab === 'pos' ? 'active' : ''}`}
            onClick={() => setActiveTab('pos')}
          >
            New receipt
          </button>
          <button
            className={`tab-button ${activeTab === 'history' ? 'active' : ''}`}
            onClick={() => setActiveTab('history')}
          >
            Sales history
          </button>
        </div>
      </div>

      {loading ? (
        <div style={{ textAlign: 'center', padding: '60px', color: 'var(--text-secondary)' }}>
          Loading data...
        </div>
      ) : (
        <>
          {activeTab === 'pos' && (
            <div className="pos-container">
              <div className="pos-left">
                <form onSubmit={handleAddBySku} style={{ marginBottom: '20px' }}>
                  <div style={{ position: 'relative' }}>
                    <input
                      type="text"
                      value={skuSearch}
                      onChange={handleSearchChange}
                      placeholder="Enter the SKU, barcode or product name..."
                      className="input"
                      autoFocus
                      style={{ width: '100%', padding: '14px', fontSize: '16px' }}
                    />
                    {suggestions.length > 0 && (
                      <ul style={{
                        position: 'absolute',
                        top: '100%',
                        left: 0,
                        right: 0,
                        background: 'var(--bg-secondary)',
                        border: '1px solid var(--border-color)',
                        borderRadius: '0 0 12px 12px',
                        zIndex: 100,
                        maxHeight: '320px',
                        overflowY: 'auto'
                      }}>
                        {suggestions.map(p => (
                          <li
                            key={p.id}
                            onClick={() => addProductToCart(p)}
                            style={{
                              padding: '12px 16px',
                              cursor: 'pointer',
                              borderBottom: '1px solid var(--border-color)'
                            }}
                          >
                            <div><b>{p.name}</b> <small style={{ color: 'var(--text-secondary)' }}>({p.sku})</small></div>
                            <div style={{ color: 'var(--success)' }}>{p.defaultSalePrice} ₴</div>
                          </li>
                        ))}
                      </ul>
                    )}
                  </div>
                </form>

                <h3>Current receipt</h3>
                <div className="table-container">
                  <table>
                    <thead>
                      <tr>
                        <th>SKU</th>
                        <th>Product name</th>
                        <th>Quantity</th>
                        <th>In stock</th>
                        <th>Price</th>
                        <th>Total</th>
                        <th></th>
                      </tr>
                    </thead>
                    <tbody>
                      {cart.map((item, idx) => {
                        const shortage = item.quantity > item.stock;
                        return (
                          <tr key={item.productId} style={{ backgroundColor: shortage ? '#450a0a' : 'transparent' }}>
                            <td><code>{item.sku}</code></td>
                            <td>{item.name}</td>
                            <td>
                              <input
                                type="number"
                                min="1"
                                value={item.quantity}
                                onChange={(e) => updateQuantity(idx, e.target.value)}
                                style={{ width: '80px', padding: '6px', textAlign: 'center' }}
                              />
                            </td>
                            <td style={{ color: item.stock === 0 ? 'var(--danger)' : 'inherit' }}>
                              {item.stock}
                            </td>
                            <td>{item.defaultSalePrice} ₴</td>
                            <td><b>{(item.quantity * item.defaultSalePrice).toFixed(2)} ₴</b></td>
                            <td>
                              <button onClick={() => removeFromCart(idx)} className="btn btn-danger" style={{ padding: '6px 10px' }}>
                                Delete
                              </button>
                            </td>
                          </tr>
                        );
                      })}
                      {cart.length === 0 && (
                        <tr>
                          <td colSpan="7" style={{ textAlign: 'center', padding: '40px', color: 'var(--text-secondary)' }}>
                            The receipt is blank. Add items to start the sale.
                          </td>
                        </tr>
                      )}
                    </tbody>
                  </table>
                </div>
              </div>
              <div className="pos-right">
                <h3>In summary</h3>
                <div style={{ fontSize: '28px', fontWeight: 'bold', margin: '20px 0', textAlign: 'center' }}>
                  {totalSum.toFixed(2)} ₴
                </div>

                {hasStockError && (
                  <div style={{ background: '#450a0a', color: '#ff9999', padding: '12px', borderRadius: '10px', marginBottom: '16px' }}>
                    We are out of stock on some items
                  </div>
                )}

                <button
                  onClick={handleCheckout}
                  disabled={cart.length === 0 || hasStockError}
                  className="btn btn-success"
                  style={{ width: '100%', padding: '16px', fontSize: '18px' }}
                >
                  Process transaction
                </button>
              </div>
            </div>
          )}

          {activeTab === 'history' && (
            <div className="table-container">
              <div style={{ padding: '20px 24px', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <h3>Sales History</h3>
                <input
                  type="text"
                  placeholder="Search by receipt number..."
                  value={searchReceiptId}
                  onChange={(e) => setSearchReceiptId(e.target.value)}
                  style={{
                    padding: '10px 14px',
                    width: '320px',
                    background: 'var(--bg-secondary)',
                    border: '1px solid var(--border-color)',
                    borderRadius: '10px',
                    color: 'var(--text-primary)'
                  }}
                />
              </div>

              <table>
                <thead>
                  <tr>
                    <th>Receipt number</th>
                    <th>Date</th>
                    <th>Number of items</th>
                    <th>Sum</th>
                    <th>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {filteredHistory.map(receipt => (
                    <tr key={receipt.id} style={{ backgroundColor: selectedReceipt?.id === receipt.id ? '#1e3a5f' : 'transparent' }}>
                      <td><code>{receipt.receiptNumber || receipt.id}</code></td>
                      <td>{new Date(receipt.saleDate).toLocaleString('uk-UA')}</td>
                      <td>{receipt.items?.length || 0}</td>
                      <td>
                        <b>{receipt.totalAmount} ₴</b>
                      </td>
                      <td>
                        <button
                          onClick={() => openReceiptDetails(receipt)}
                          className="btn btn-primary"
                          style={{ marginRight: '8px' }}
                        >
                          View products
                        </button>
                        <button
                          onClick={() => openReturnModal(receipt)}
                          className="btn"
                          style={{ background: '#f59e0b' }}
                        >
                          Return
                        </button>
                      </td>
                    </tr>
                  ))}
                  {filteredHistory.length === 0 && (
                    <tr>
                      <td colSpan="5" style={{ padding: '40px', textAlign: 'center', color: 'var(--text-secondary)' }}>
                        No checks were found
                      </td>
                    </tr>
                  )}
                </tbody>
              </table>

              {selectedReceipt && (
                <div style={{ marginTop: '30px', padding: '24px', background: 'var(--bg-card)', borderRadius: '16px', border: '1px solid var(--border-color)' }}>
                  <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
                    <h3>Check details: {selectedReceipt.receiptNumber || selectedReceipt.id}</h3>
                    <button className="btn" onClick={() => setSelectedReceipt(null)}>Hide details</button>
                  </div>

                  <table style={{ width: '100%' }}>
                    <thead>
                      <tr>
                        <th style={{ textAlign: 'left' }}>Product</th>
                        <th>Quantity</th>
                        <th>Price per unit</th>
                        <th>Total</th>
                      </tr>
                    </thead>
                    <tbody>
                      {selectedReceipt.items.map((item, index) => {
                        const qty = item.quantity;
                        const totalItemPrice = item.total;
                        return (
                          <tr key={index}>
                            <td>{item.productName}</td>
                            <td style={{ textAlign: 'center' }}>{qty}</td>
                            <td>{item.price} ₴</td>
                            <td><b>{totalItemPrice} ₴</b></td>
                          </tr>
                        );
                      })}
                    </tbody>
                  </table>

                  <div style={{ marginTop: '20px', fontSize: '18px', textAlign: 'right' }}>
                    <strong>Total amount: {selectedReceipt.totalAmount} ₴</strong>
                  </div>
                </div>
              )}
            </div>
          )}
        </>
      )}

      {returnModal.open && (
        <div className="modal-overlay">
          <div className="modal">
            <h3>Processing a return</h3>
            <p style={{ color: 'var(--text-secondary)', marginBottom: '20px' }}>
              Receipt: <code>{returnModal.receipt?.receiptNumber || returnModal.receipt?.id}</code>
            </p>

            <form onSubmit={submitReturn}>
              <table style={{ width: '100%', borderCollapse: 'collapse' }}>
                <thead>
                  <tr>
                    <th style={{ textAlign: 'left' }}>Product</th>
                    <th>Purchased</th>
                    <th>Return</th>
                  </tr>
                </thead>
                <tbody>
                  {returnModal.items.map((item, index) => (
                    <tr key={index}>
                      <td>{item.name}</td>
                      <td style={{ textAlign: 'center' }}>{item.maxQuantity}</td>
                      <td>
                        <input
                          type="number"
                          min="0"
                          max={item.maxQuantity}
                          value={item.quantity}
                          onChange={(e) => handleReturnQtyChange(index, e.target.value)}
                          style={{ width: '100px', padding: '8px', textAlign: 'center' }}
                        />
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>

              <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '12px', marginTop: '24px' }}>
                <button type="button" className="btn" onClick={() => setReturnModal({ open: false, receipt: null, items: [] })}>
                  Cancel
                </button>
                <button type="submit" className="btn btn-danger">
                  Confirm the return
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};

export default SalesManagement;