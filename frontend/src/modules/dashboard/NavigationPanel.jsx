import React from 'react';
import { useNavigate } from 'react-router-dom';

const NavigationPanel = ({ user }) => {
    const navigate = useNavigate();

    const hasRole = role =>
        user?.roles?.includes(role);

    return (
        <section className="navigation-panel">

            <div className="navigation-header">
                <h2>System Navigation</h2>
                <span>
                    Roles: {user?.roles?.join(', ')}
                </span>
            </div>

            <div className="navigation-grid">
                {(hasRole('Seller') || hasRole('Admin')) && (
                <button
                    className="nav-card"
                    onClick={() => navigate('/sales')}
                >
                    <h3>Sales</h3>
                    <p>Sales workstation</p>
                </button>
                )}

                {(hasRole('WarehouseAdmin') || hasRole('Admin')) && (
                <button
                    className="nav-card"
                    onClick={() => navigate('/stock')}
                >
                    <h3>Storage</h3>
                    <p>Inventory Management</p>
                </button>
                )}

                {(hasRole('Manager') || hasRole('Admin')) && (
                <button
                    className="nav-card"
                    onClick={() => navigate('/reports')}
                >
                    <h3>Reports</h3>
                    <p>Analytics and Statistics</p>
                </button>
                )}
                {(hasRole('Manager') || hasRole('Admin')) && (
                <button
                    className="nav-card"
                    onClick={() => navigate('/feedback-management')}
                >
                    <h3>Feedback</h3>
                    <p>Users feedback</p>
                </button>
                )}
                {hasRole('Admin') && (
                    <>
                        <button
                            className="nav-card admin-card"
                            onClick={() => navigate('/admin/users')}
                        >
                            <h3>Users</h3>
                            <p>Account Management</p>
                        </button>

                        <button
                            className="nav-card admin-card"
                            onClick={() => navigate('/catalog/settings')}
                        >
                            <h3>Catalog</h3>
                            <p>Product range configuration</p>
                        </button>
                    </>
                )}

            </div>

        </section>
    );
};

export default NavigationPanel;