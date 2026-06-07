import React, { useState, useEffect } from 'react';
import { catalogService } from '../../services/catalogService';
import './CatalogManagement.css';

import TabNavigation from './TabNavigation';
import ProductsTable from './ProductsTable';
import CategoriesTable from './CategoriesTable';
import ClassesTable from './ClassesTable';

import CreateProductModal from './modals/CreateProductModal';
import EditProductModal from './modals/EditProductModal';
import CreateCategoryModal from './modals/CreateCategoryModal';
import EditCategoryModal from './modals/EditCategoryModal';
import CreateClassModal from './modals/CreateClassModal';
import EditClassModal from './modals/EditClassModal';




const CatalogManagement = () => {
  const [activeTab, setActiveTab] = useState('products');
  const [classes, setClasses] = useState([]);
  const [categories, setCategories] = useState([]);
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [includeInactive, setIncludeInactive] = useState(true);

  const [modals, setModals] = useState({
    createClass: false, editClass: false,
    createCategory: false, editCategory: false,
    createProduct: false, editProduct: false
  });

  const [selectedEntity, setSelectedEntity] = useState({ id: null, version: 0 });

  const [classCreateForm, setClassCreateForm] = useState({ code: '', name: '', description: '' });
  const [classEditForm, setClassEditForm] = useState({ description: '' });
  const [categoryCreateForm, setCategoryCreateForm] = useState({ parentId: '', code: '', name: '', description: '' });
  const [categoryEditForm, setCategoryEditForm] = useState({ description: '' });
  const [productCreateForm, setProductCreateForm] = useState({
    sku: '', barcode: '', name: '', classId: '', categoryId: '', unit: 'шт',
    defaultShelfLifeDays: '', defaultPurchasePrice: '', defaultSalePrice: '1.00'
  });
  const [productEditForm, setProductEditForm] = useState({ name: '', classId: '', categoryId: '', unit: 'шт' });

  const [attributes, setAttributes] = useState([]);
  const [newKey, setNewKey] = useState('');
  const [newValue, setNewValue] = useState('');

  const loadData = async () => {
      try {
        setLoading(true);
        const [resClasses, resCategories, resProducts] = await Promise.all([
          catalogService.getClasses(includeInactive),
          catalogService.getCategories(includeInactive),
          catalogService.getProducts(includeInactive)
        ]);

        setClasses(resClasses.data || []);
        setCategories(resCategories.data || []);
        setProducts(resProducts.data || []);
      } catch (err) {
        alert('Loading error: ' + (err.response?.data?.message || err.message));
      } finally {
        setLoading(false);
      }
    };

    useEffect(() => {
      loadData();
    }, [includeInactive]);

  const handleToggleStatus = async (type, item) => {
    const isActivating = !item.isActive;
    try {
      if (type === 'product') {
        isActivating 
          ? await catalogService.activateProduct(item.id, item.version)
          : await catalogService.deactivateProduct(item.id, item.version);
      } else if (type === 'category') {
        isActivating 
          ? await catalogService.activateCategory(item.id, item.version)
          : await catalogService.deactivateCategory(item.id, item.version);
      } else if (type === 'class') {
        isActivating 
          ? await catalogService.activateClass(item.id, item.version)
          : await catalogService.deactivateClass(item.id, item.version);
      }
      alert('Status successfully updated');
      loadData();
    } catch (err) {
      alert('Status change error: ' + (err.response?.data?.message || err.message));
    }
  };

  const handleCreateClass = async (e) => {
    e.preventDefault();
    try {
      await catalogService.createClass(classCreateForm);
      setModals({ ...modals, createClass: false });
      setClassCreateForm({ code: '', name: '', description: '' });
      loadData();
    } catch (err) { alert(err.response?.data?.message || 'Error'); }
  };

  const handleEditClass = async (e) => {
    e.preventDefault();
    try {
      await catalogService.updateClassDescription(selectedEntity.id, { description: classEditForm.description, version: selectedEntity.version });
      setModals({ ...modals, editClass: false });
      loadData();
    } catch (err) { alert(err.response?.data?.message || 'Error'); }
  };

  const handleCreateCategory = async (e) => {
    e.preventDefault();
    try {
      const payload = { ...categoryCreateForm, parentId: categoryCreateForm.parentId || null };
      await catalogService.createCategory(payload);
      setModals({ ...modals, createCategory: false });
      setCategoryCreateForm({ parentId: '', code: '', name: '', description: '' });
      loadData();
    } catch (err) { alert(err.response?.data?.message || 'Error'); }
  };

  const handleEditCategory = async (e) => {
    e.preventDefault();
    try {
      await catalogService.updateCategoryDescription(selectedEntity.id, { description: categoryEditForm.description, version: selectedEntity.version });
      setModals({ ...modals, editCategory: false });
      loadData();
    } catch (err) { alert(err.response?.data?.message || 'Error'); }
  };

  const handleCreateProduct = async (e) => {
    e.preventDefault();
    try {
      const attrsObj = {}; 
      attributes.forEach(a => { attrsObj[a.key] = a.value; });
      
      const payload = {
        ...productCreateForm,
        defaultShelfLifeDays: productCreateForm.defaultShelfLifeDays ? parseInt(productCreateForm.defaultShelfLifeDays) : null,
        defaultPurchasePrice: productCreateForm.defaultPurchasePrice ? parseFloat(productCreateForm.defaultPurchasePrice) : null,
        defaultSalePrice: parseFloat(productCreateForm.defaultSalePrice),
        attributes: attrsObj
      };

      await catalogService.createProduct(payload);
      setModals({ ...modals, createProduct: false });
      setProductCreateForm({ 
        sku: '', barcode: '', name: '', classId: '', categoryId: '', unit: 'pcx',
        defaultShelfLifeDays: '', defaultPurchasePrice: '', defaultSalePrice: '1.00'
      });
      setAttributes([]);
      loadData();
    } catch (err) { alert(err.response?.data?.message || 'Error'); }
  };

  const handleEditProduct = async (e) => {
    e.preventDefault();
    try {
      const attrsObj = {}; attributes.forEach(a => { attrsObj[a.key] = a.value; });
      await catalogService.updateProduct(selectedEntity.id, { ...productEditForm, version: selectedEntity.version, attributes: attrsObj });
      setModals({ ...modals, editProduct: false });
      loadData();
    } catch (err) { alert(err.response?.data?.message || 'Error'); }
  };

  const addAttributeRow = () => {
    if (!newKey.trim() || !newValue.trim()) return;
    setAttributes([...attributes, { key: newKey.trim(), value: newValue.trim() }]);
    setNewKey('');
    setNewValue('');
  };

  const removeAttributeRow = (index) => {
    setAttributes(attributes.filter((_, i) => i !== index));
  };

  return (
    
    <div className="catalog-container">
      <div className="catalog-header">
        <h2>Product Catalog Management</h2>
        <label style={{ display: 'flex', alignItems: 'center', gap: '8px', cursor: 'pointer' }}>
          <input
            type="checkbox"
            checked={includeInactive}
            onChange={e => setIncludeInactive(e.target.checked)}
          />
          Show inactive data
        </label>
      </div>

      <TabNavigation activeTab={activeTab} setActiveTab={setActiveTab} />

      {loading ? (
        <div style={{ textAlign: 'center', padding: '60px', color: 'var(--text-secondary)' }}>
          Loading data...
        </div>
      ) : (
        <div className="table-container">
          {activeTab === 'products' && (
            <ProductsTable
              products={products}
              classes={classes}
              categories={categories}
              onEdit={(product) => {
                setSelectedEntity({ id: product.id, version: product.version });
                setProductEditForm({
                  name: product.name,
                  classId: product.classId,
                  categoryId: product.categoryId,
                  unit: product.unit
                });
                setAttributes(Object.entries(product.attributes || {}).map(([key, value]) => ({ key, value })));
                setModals(prev => ({ ...prev, editProduct: true }));
              }}
              onToggleStatus={(item) => handleToggleStatus('product', item)}
              onCreate={() => {
                setAttributes([]);
                setModals(prev => ({ ...prev, createProduct: true }));
              }}
            />
          )}

          {activeTab === 'categories' && (
            <CategoriesTable
              categories={categories}
              onEdit={(cat) => {
                setSelectedEntity({ id: cat.id, version: cat.version });
                setCategoryEditForm({ description: cat.description || '' });
                setModals(prev => ({ ...prev, editCategory: true }));
              }}
              onToggleStatus={(item) => handleToggleStatus('category', item)}
              onCreate={() => setModals(prev => ({ ...prev, createCategory: true }))}
            />
          )}

          {activeTab === 'classes' && (
            <ClassesTable
              classes={classes}
              onEdit={(cl) => {
                setSelectedEntity({ id: cl.id, version: cl.version });
                setClassEditForm({ description: cl.description || '' });
                setModals(prev => ({ ...prev, editClass: true }));
              }}
              onToggleStatus={(item) => handleToggleStatus('class', item)}
              onCreate={() => setModals(prev => ({ ...prev, createClass: true }))}
            />
          )}
        </div>
      )}

    
      <CreateProductModal
        isOpen={modals.createProduct}
        onClose={() => setModals(prev => ({ ...prev, createProduct: false }))}
        form={productCreateForm}
        setForm={setProductCreateForm}
        classes={classes}
        categories={categories}
        attributes={attributes}
        setAttributes={setAttributes}
        newKey={newKey}
        setNewKey={setNewKey}
        newValue={newValue}
        setNewValue={setNewValue}
        onSubmit={handleCreateProduct}
        onAddAttribute={addAttributeRow}
        onRemoveAttribute={removeAttributeRow}
      />

      <EditProductModal
        isOpen={modals.editProduct}
        onClose={() => setModals(prev => ({ ...prev, editProduct: false }))}
        form={productEditForm}
        setForm={setProductEditForm}
        classes={classes}
        categories={categories}
        attributes={attributes}
        setAttributes={setAttributes}
        newKey={newKey}
        setNewKey={setNewKey}
        newValue={newValue}
        setNewValue={setNewValue}
        onSubmit={handleEditProduct}
        onAddAttribute={addAttributeRow}
        onRemoveAttribute={removeAttributeRow}
      />

      <CreateCategoryModal
        isOpen={modals.createCategory}
        onClose={() => setModals(prev => ({ ...prev, createCategory: false }))}
        form={categoryCreateForm}
        setForm={setCategoryCreateForm}
        categories={categories}
        onSubmit={handleCreateCategory}
      />

      <EditCategoryModal
        isOpen={modals.editCategory}
        onClose={() => setModals(prev => ({ ...prev, editCategory: false }))}
        form={categoryEditForm}
        setForm={setCategoryEditForm}
        onSubmit={handleEditCategory}
      />

      <CreateClassModal
        isOpen={modals.createClass}
        onClose={() => setModals(prev => ({ ...prev, createClass: false }))}
        form={classCreateForm}
        setForm={setClassCreateForm}
        onSubmit={handleCreateClass}
      />

      <EditClassModal
        isOpen={modals.editClass}
        onClose={() => setModals(prev => ({ ...prev, editClass: false }))}
        form={classEditForm}
        setForm={setClassEditForm}
        onSubmit={handleEditClass}
      />
    </div>
  );
  
};

export default CatalogManagement;