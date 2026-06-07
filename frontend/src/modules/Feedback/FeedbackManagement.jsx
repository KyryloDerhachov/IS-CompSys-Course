import React, { useState, useEffect } from 'react';
import { feedbackService } from '../../services/feedbackService';
import styles from './FeedbackManagement.module.css';

const FEEDBACK_TYPES = {
  1: 'Suggestion',
  2: 'Complaint',
};

const FeedbackManagement = () => {
  const [feedbacks, setFeedbacks] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [selectedFeedbackId, setSelectedFeedbackId] = useState(null);
  const [managerResponse, setManagerResponse] = useState('');
  const [submitting, setSubmitting] = useState(false);

  const loadFeedbacks = async () => {
    try {
      setLoading(true);
      setError('');
      const response = await feedbackService.getAllFeedbacks();
      setFeedbacks(response.data);
    } catch (err) {
      setError('Failed to load feedback data from server.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadFeedbacks();
  }, []);

  const handleOpenRespondModal = (feedback) => {
    setSelectedFeedbackId(feedback.id);
    setManagerResponse(feedback.managerResponse || '');
    setIsModalOpen(true);
  };

  const handleRespondSubmit = async (e) => {
    e.preventDefault();
    try {
      setSubmitting(true);
      await feedbackService.respondToFeedback(selectedFeedbackId, managerResponse);
      
      alert('Response submitted successfully.');
      setIsModalOpen(false);
      loadFeedbacks();
    } catch (err) {
      alert(err.response?.data?.message || 'An error occurred while saving the response.');
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className={styles.reportsContainer}>
      <div className={styles.reportsCard}>
        <div className={styles.headerPanel}>
          <h2>Manager Panel: Feedback Administration</h2>
        </div>

        {error && <div className={styles.alertMessage}>{error}</div>}

        {loading ? (
          <div className={styles.loadingContainer}>Loading feedback records...</div>
        ) : (
          <div className={styles.tableWrapper}>
            <table className={styles.reportsTable}>
              <thead>
                <tr>
                  <th>Receipt No.</th>
                  <th>Type</th>
                  <th>Content</th>
                  <th>Status</th>
                  <th>Manager Response</th>
                  <th>Created At</th>
                  <th>Action</th>
                </tr>
              </thead>
              <tbody>
                {feedbacks.map(f => (
                  <tr key={f.id}>
                    <td><strong>{f.receiptNumber}</strong></td>
                    <td>
                      <span className={styles.roleBadge}>
                        {FEEDBACK_TYPES[f.type] || 'Unknown'}
                      </span>
                    </td>
                    <td className={styles.truncateCell} title={f.content}>
                      {f.content}
                    </td>
                    <td>
                      {f.managerResponse ? (
                        <span className={`${styles.statusIndicator} ${styles.statusActive}`}>
                          Responded
                        </span>
                      ) : (
                        <span className={`${styles.statusIndicator} ${styles.statusInactive}`}>
                          Pending
                        </span>
                      )}
                    </td>
                    <td className={styles.truncateCell} title={f.managerResponse || ''}>
                      {f.managerResponse ? (
                        f.managerResponse
                      ) : (
                        <i style={{ color: '#64748b' }}>No response yet</i>
                      )}
                    </td>
                    <td>{new Date(f.createdAt).toLocaleDateString()}</td>
                    <td>
                      <button
                        onClick={() => handleOpenRespondModal(f)}
                        className={`${styles.btn} ${styles.btnWarning}`}
                      >
                        {f.managerResponse ? 'Edit Response' : 'Respond'}
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
      {isModalOpen && (
        <div className={styles.modalOverlay}>
          <div className={styles.modalContent}>
            <h3>Provide Manager Response</h3>
            
            <form onSubmit={handleRespondSubmit}>
              <div className={styles.formGroup}>
                <label>Response Message</label>
                <textarea
                  value={managerResponse}
                  onChange={e => setManagerResponse(e.target.value)}
                  required
                  rows={6}
                  placeholder="Type your official response here..."
                  className={styles.filterInput}
                  style={{ resize: 'vertical' }}
                />
              </div>

              <div className={styles.modalActions}>
                <button
                  type="button"
                  onClick={() => setIsModalOpen(false)}
                  className={`${styles.btn} ${styles.btnSecondary}`}
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  disabled={submitting}
                  className={`${styles.btn} ${styles.btnInfo}`}
                >
                  {submitting ? 'Saving...' : 'Submit Response'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};

export default FeedbackManagement;