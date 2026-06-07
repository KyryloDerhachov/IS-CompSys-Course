import React, { useState } from 'react';
import { feedbackService } from '../../services/feedbackService';
import styles from './FeedbackForm.module.css';

const FEEDBACK_TYPES = {
1: 'Suggestion',
2: 'Complaint'
};

const FeedbackForm = () => {
const [loading, setLoading] = useState(false);
const [success, setSuccess] = useState(false);
const [error, setError] = useState('');

const [formData, setFormData] = useState({
receiptNumber: '',
type: 1,
content: ''
});

const handleSubmit = async (e) => {
e.preventDefault();


try {
  setLoading(true);
  setError('');
  setSuccess(false);

  await feedbackService.createFeedback({
    receiptNumber: formData.receiptNumber,
    type: Number(formData.type),
    content: formData.content
  });

  setSuccess(true);

  setFormData({
    receiptNumber: '',
    type: 1,
    content: ''
  });
} catch (err) {
  setError(
    err.response?.data?.message ||
    'Something went wrong. Please try again.'
  );
} finally {
  setLoading(false);
}


};

return ( 
    <div className={styles.reportsContainer}> <div className={styles.reportsCard}> <div className={styles.headerPanel}> <h2>Submit Your Feedback</h2> <p>
    Share your suggestions or report issues related to your purchase. </p> </div>
    {error && (
      <div className={styles.alertMessageDanger}>
        {error}
      </div>
    )}

    {success && (
      <div className={styles.alertMessageSuccess}>
        Thank you! Your feedback has been submitted successfully.
      </div>
    )}

    <form onSubmit={handleSubmit}>
      <div className={styles.formGroup}>
        <label htmlFor="receiptNumber">
          Receipt Number
        </label>

        <input
          id="receiptNumber"
          type="text"
          placeholder="e.g. REC-123456"
          value={formData.receiptNumber}
          onChange={(e) =>
            setFormData({
              ...formData,
              receiptNumber: e.target.value
            })
          }
          className={styles.filterInput}
          required
        />
      </div>

      <div className={styles.formGroup}>
        <label htmlFor="type">
          Feedback Type
        </label>

        <select
          id="type"
          value={formData.type}
          onChange={(e) =>
            setFormData({
              ...formData,
              type: e.target.value
            })
          }
          className={styles.filterInput}
        >
          {Object.entries(FEEDBACK_TYPES).map(([value, label]) => (
            <option
              key={value}
              value={value}
            >
              {label}
            </option>
          ))}
        </select>
      </div>

      <div className={styles.formGroup}>
        <label htmlFor="content">
          Content
        </label>

        <textarea
          id="content"
          rows={6}
          placeholder="Please describe your experience..."
          value={formData.content}
          onChange={(e) =>
            setFormData({
              ...formData,
              content: e.target.value
            })
          }
          className={styles.filterInput}
          required
        />
      </div>

      <div className={styles.modalActions}>
        <button
          type="submit"
          disabled={loading}
          className={`${styles.btn} ${styles.btnInfo}`}
        >
          {loading ? 'Submitting...' : 'Submit Feedback'}
        </button>
      </div>
    </form>
  </div>
</div>

);
};

export default FeedbackForm;
