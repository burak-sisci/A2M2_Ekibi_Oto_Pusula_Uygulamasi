import { useState, useEffect } from 'react';
import { getComments, addComment, updateComment, deleteComment } from '../../api/commentApi';
import { useAuth } from '../../context/AuthContext';
import { toast } from 'react-toastify';
import { FaEdit, FaTrash, FaPaperPlane } from 'react-icons/fa';
import './Comments.css';

const CommentSection = ({ carId }) => {
  const { user } = useAuth();
  const [comments, setComments] = useState([]);
  const [content, setContent] = useState('');
  const [editingId, setEditingId] = useState(null);
  const [editContent, setEditContent] = useState('');
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(0);
  const [totalCount, setTotalCount] = useState(0);
  const limit = 10;

  const fetchComments = async () => {
    try {
      const res = await getComments(carId, { limit, offset: page * limit });
      setComments(res.data.data || []);
      setTotalCount(res.data.totalCount || 0);
    } catch {
      // silently fail
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchComments();
  }, [carId, page]);

  const handleAdd = async (e) => {
    e.preventDefault();
    if (!content.trim()) return;
    try {
      await addComment(carId, { content });
      setContent('');
      fetchComments();
      toast.success('Yorum eklendi');
    } catch (err) {
      toast.error(err.response?.data?.message || 'Yorum eklenemedi');
    }
  };

  const handleUpdate = async (commentId) => {
    try {
      await updateComment(carId, commentId, { content: editContent });
      setEditingId(null);
      fetchComments();
      toast.success('Yorum guncellendi');
    } catch (err) {
      toast.error(err.response?.data?.message || 'Guncelleme basarisiz');
    }
  };

  const handleDelete = async (commentId) => {
    if (!window.confirm('Yorumu silmek istiyor musunuz?')) return;
    try {
      await deleteComment(carId, commentId);
      fetchComments();
      toast.success('Yorum silindi');
    } catch (err) {
      toast.error(err.response?.data?.message || 'Silme basarisiz');
    }
  };

  const totalPages = Math.ceil(totalCount / limit);

  return (
    <div className="comment-section">
      <h3>Yorumlar ({totalCount})</h3>

      {user && (
        <form className="comment-form" onSubmit={handleAdd}>
          <input
            value={content}
            onChange={(e) => setContent(e.target.value)}
            placeholder="Yorumunuzu yazin..."
            required
          />
          <button type="submit" className="btn-send">
            <FaPaperPlane />
          </button>
        </form>
      )}

      {loading ? (
        <div className="loading"><div className="spinner"></div></div>
      ) : comments.length === 0 ? (
        <p className="no-comments">Henuz yorum yok</p>
      ) : (
        <div className="comments-list">
          {comments.map((c) => (
            <div key={c.id} className="comment-item">
              {editingId === c.id ? (
                <div className="comment-edit">
                  <input
                    value={editContent}
                    onChange={(e) => setEditContent(e.target.value)}
                  />
                  <button className="btn btn-primary" onClick={() => handleUpdate(c.id)} style={{ width: 'auto', padding: '8px 16px', fontSize: '0.85rem' }}>
                    Kaydet
                  </button>
                  <button className="btn btn-secondary" onClick={() => setEditingId(null)} style={{ width: 'auto', padding: '8px 16px', fontSize: '0.85rem' }}>
                    Iptal
                  </button>
                </div>
              ) : (
                <>
                  <div className="comment-content">
                    <p>{c.content}</p>
                    <span className="comment-date">
                      {new Date(c.createdAt).toLocaleDateString('tr-TR')}
                    </span>
                  </div>
                  {user && c.userId === user.id && (
                    <div className="comment-actions">
                      <button onClick={() => { setEditingId(c.id); setEditContent(c.content); }}>
                        <FaEdit />
                      </button>
                      <button onClick={() => handleDelete(c.id)}>
                        <FaTrash />
                      </button>
                    </div>
                  )}
                </>
              )}
            </div>
          ))}
        </div>
      )}

      {totalPages > 1 && (
        <div className="pagination" style={{ marginTop: 16 }}>
          <button disabled={page === 0} onClick={() => setPage(page - 1)} className="btn btn-secondary">Onceki</button>
          <span className="page-info">{page + 1} / {totalPages}</span>
          <button disabled={page >= totalPages - 1} onClick={() => setPage(page + 1)} className="btn btn-secondary">Sonraki</button>
        </div>
      )}
    </div>
  );
};

export default CommentSection;
