import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import ProtectedRoute from './components/ProtectedRoute';
import Login from './modules/auth/Login';
import Dashboard from './modules/dashboard/Dashboard';
import UsersManagement from './modules/admin/UsersManagement';
import CatalogManagement from './modules/catalog/CatalogManagement';
import StockManagement from './modules/stock/StockManagement';
import SalesManagement from './modules/sales/SalesManagement';
import ReportsManagement from './modules/reports/ReportsManagement';
import FeedbackForm from './modules/Feedback/FeedbackForm';
import FeedbackManagement from './modules/Feedback/FeedbackManagement';


const Unauthorized = () => <h2 style={{ color: 'red', padding: '20px' }}>403 - Доступ заборонено (Недостатньо прав ролі)</h2>;


function App() {
  return (
    <AuthProvider>
      <Router>
        <Routes>
          
          <Route path="/login" element={<Login />} />
          <Route path="/unauthorized" element={<Unauthorized />} />
          <Route path="/feedback" element={<FeedbackForm />} />
         
          <Route element={<ProtectedRoute />}>
            <Route path="/" element={<Dashboard />} />
          </Route>

          <Route element={<ProtectedRoute allowedRoles={['Seller', 'Admin']} />}>
            <Route path="/sales" element={<SalesManagement />} />
          </Route>

          <Route element={<ProtectedRoute allowedRoles={['Manager', 'Admin']} />}>
            <Route path="/reports" element={<ReportsManagement/>} />
          </Route>

          <Route element={<ProtectedRoute allowedRoles={['Manager', 'Admin']} />}>
            <Route path="/feedback-management" element={<FeedbackManagement/>} />
          </Route>

          <Route element={<ProtectedRoute allowedRoles={['WarehouseAdmin', 'Admin']} />}>
            <Route path="/stock" element={<StockManagement />} />
          </Route>

          <Route element={<ProtectedRoute allowedRoles={['Admin']} />}>
            <Route path="/admin/users" element={<UsersManagement />} />
            <Route path="/catalog/settings" element={<CatalogManagement />} />
          </Route>

          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </Router>
    </AuthProvider>
  );
}

export default App;