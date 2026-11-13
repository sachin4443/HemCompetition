
export default function RulesModal({ show, onClose, gameDetails, onStartGame }) {
  if (!show || !gameDetails) return null;

  return (
    <div className="modal fade show d-block" style={{ backgroundColor: 'rgba(0, 0, 0, 0.5)' }} tabIndex="-1">
      <div className="modal-dialog modal-dialog-centered">
        <div className="modal-content border-0 shadow-lg rounded-4">
          <div className="modal-header border-0 pb-0">
            <h5 className="modal-title fw-bold">Rules - {gameDetails.title}</h5>
            <button type="button" className="btn-close" onClick={onClose}></button>
          </div>
          <div className="modal-body py-4">
            <ol className="mb-0 ps-3">
              {gameDetails.rules?.map((rule, idx) => (
                <li key={idx} className="mb-2 text-secondary">{rule}</li>
              ))}
            </ol>
          </div>
          <div className="modal-footer border-0 pt-0 d-flex justify-content-end gap-2">
            <button type="button" className="btn btn-secondary px-4 rounded-pill" onClick={onClose}>
              Close
            </button>
            <button type="button" className="btn btn-primary px-4 rounded-pill fw-bold" onClick={onStartGame}>
              Start Game
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}