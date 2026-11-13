import React, { useEffect, useState } from "react";
import { Document, Page, pdfjs } from "react-pdf";

import "react-pdf/dist/Page/TextLayer.css";
import "react-pdf/dist/Page/AnnotationLayer.css";

pdfjs.GlobalWorkerOptions.workerSrc = new URL(
    "pdfjs-dist/build/pdf.worker.min.mjs",
    import.meta.url
).toString();

export default function PdfViewerModal({ show, onClose }) {
    const [numPages, setNumPages] = useState(0);
    const [pageNumber, setPageNumber] = useState(1);
    const [scale, setScale] = useState(1.0);

    useEffect(() => {
        if (show) {
            setPageNumber(1);
            setScale(1.0);
        }
    }, [show]);

    if (!show) return null;

    return (
        <div className="modal fade show d-block" tabIndex="-1" style={{ backgroundColor: "rgba(0,0,0,0.6)" }}>
            <div className="modal-dialog modal-fullscreen m-0">
                <div className="modal-content h-100 border-0 rounded-0 d-flex flex-column">
                    
                    {/* Modal Header */}
                    <div className="modal-header bg-white shadow-sm px-4 py-3">
                        <h5 className="modal-title fw-bold text-dark">
                            📘 Competition User Guide
                        </h5>
                        <button className="btn-close" onClick={onClose} />
                    </div>

                    {/* Toolbar */}
                    <div className="bg-light border-bottom py-2 px-3 d-flex justify-content-center align-items-center flex-wrap gap-3">
                        <button
                            className="btn btn-outline-primary btn-sm px-3"
                            disabled={pageNumber <= 1}
                            onClick={() => setPageNumber((p) => Math.max(1, p - 1))}
                        >
                            ◀ Previous
                        </button>

                        <div className="d-flex align-items-center gap-2">
                            <button
                                className="btn btn-outline-secondary btn-sm"
                                onClick={() => setScale((s) => Math.max(0.6, s - 0.1))}
                            >
                                -
                            </button>
                            <span className="fw-semibold small px-2">
                                {(scale * 100).toFixed(0)}%
                            </span>
                            <button
                                className="btn btn-outline-secondary btn-sm"
                                onClick={() => setScale((s) => Math.min(2.5, s + 0.1))}
                            >
                                +
                            </button>
                        </div>

                        <button
                            className="btn btn-outline-primary btn-sm px-3"
                            disabled={pageNumber >= numPages}
                            onClick={() => setPageNumber((p) => Math.min(numPages, p + 1))}
                        >
                            Next ▶
                        </button>
                    </div>

                    {/* Modal Body / PDF Canvas Container */}
                    <div 
                        className="modal-body bg-secondary bg-opacity-15 d-flex flex-column justify-content-center align-items-center overflow-auto p-3 flex-grow-1"
                        onContextMenu={(e) => e.preventDefault()}
                    >
                        <div className="shadow bg-white p-2 rounded">
                            <Document
                                file="/Competition.pdf"
                                onLoadSuccess={({ numPages }) => setNumPages(numPages)}
                                loading={
                                    <div className="text-center p-5 fw-semibold text-muted">
                                        Loading PDF document...
                                    </div>
                                }
                            >
                                <Page pageNumber={pageNumber} scale={scale} />
                            </Document>
                        </div>
                    </div>

                    {/* Modal Footer / Page Info */}
                    <div className="modal-footer bg-white py-2 justify-content-center border-top">
                        <span className="fw-semibold text-secondary small">
                            Page {pageNumber} of {numPages}
                        </span>
                    </div>

                </div>
            </div>
        </div>
    );
}