import React from 'react';

const ProductsTable = ({ products, loading }) => {

    if (loading) {
        return (
            <div className="loading-block">
                Завантаження товарів...
            </div>
        );
    }

    return (
        <div className="table-wrapper">

            <table className="products-table">

                <thead>
                    <tr>
                        <th>SKU</th>
                        <th>Title</th>
                        <th>Barcode</th>
                        <th>Category</th>
                        <th>Unit</th>
                        <th>Selling price</th>
                        <th>Status</th>
                    </tr>
                </thead>

                <tbody>

                    {products.map(product => (
                        <tr key={product.id}>

                            <td>{product.sku}</td>

                            <td>{product.name}</td>

                            <td>{product.barcode}</td>

                            <td>{product.categoryName}</td>

                            <td>{product.unit}</td>

                            <td>
                                {product.defaultSalePrice.toFixed(2)} ₴
                            </td>

                            <td>
                                <span
                                    className={
                                        product.isActive
                                            ? 'status-active'
                                            : 'status-inactive'
                                    }
                                >
                                    {product.isActive
                                        ? 'Active'
                                        : 'Inactive'}
                                </span>
                            </td>

                        </tr>
                    ))}

                    {products.length === 0 && (
                        <tr>
                            <td colSpan="7" className="empty-row">
                                No data available
                            </td>
                        </tr>
                    )}

                </tbody>

            </table>

        </div>
    );
};

export default ProductsTable;