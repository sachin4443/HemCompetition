import React, { useEffect, useState } from "react";
import { Document, Page, pdfjs } from "react-pdf";

import "react-pdf/dist/Page/TextLayer.css";
import "react-pdf/dist/Page/AnnotationLayer.css";

pdfjs.GlobalWorkerOptions.workerSrc = new URL(
    "pdfjs-dist/build/pdf.worker.min.mjs",
    import.meta.url
).toString();

export default function PdfViewerModal({
    show,
    onClose
}) {

    const [numPages, setNumPages] = useState(0);
    const [scale, setScale] = useState(1.2);

    useEffect(() => {

        if (show) {
            setScale(1.2);
        }

    }, [show]);

    if (!show) return null;

    return (

        <div
            className="modal fade show d-block"
            tabIndex="-1"
            style={{
                backgroundColor: "rgba(0,0,0,.65)",
                zIndex: 2000
            }}
        >

            <div className="modal-dialog modal-fullscreen">

                <div className="modal-content">

                    {/* Header */}

                    <div className="modal-header">

                        <h5 className="modal-title fw-bold">

                            📘 Competition User Guide

                        </h5>

                        <button
                            className="btn-close"
                            onClick={onClose}
                        />

                    </div>

                    {/* Toolbar */}

                    <div className="border-bottom bg-white px-3 py-2">

                        <div className="d-flex justify-content-center align-items-center gap-2 flex-wrap">

                            <button
                                className="btn btn-outline-secondary"
                                onClick={() =>
                                    setScale((s) => Math.max(0.7, s - 0.1))
                                }
                            >
                                −
                            </button>

                            <span
                                className="fw-semibold"
                                style={{ minWidth: 70, textAlign: "center" }}
                            >
                                {(scale * 100).toFixed(0)}%
                            </span>

                            <button
                                className="btn btn-outline-secondary"
                                onClick={() =>
                                    setScale((s) => Math.min(2.5, s + 0.1))
                                }
                            >
                                +
                            </button>

                            <button
                                className="btn btn-outline-primary"
                                onClick={() => setScale(1.2)}
                            >
                                Reset Zoom
                            </button>

                            <span className="text-muted ms-3">

                                Total Pages : {numPages}

                            </span>

                        </div>

                    </div>

                    {/* PDF */}

                    <div
                        className="modal-body bg-light overflow-auto"
                        style={{
                            height: "calc(100vh - 120px)"
                        }}
                        onContextMenu={(e) => e.preventDefault()}
                    >

                        <div className="d-flex justify-content-center">

                            <Document
                                file="/Competition.pdf"
                                loading={
                                    <div className="text-center py-5">
                                        Loading PDF...
                                    </div>
                                }
                                onLoadSuccess={({ numPages }) =>
                                    setNumPages(numPages)
                                }
                            >

                                {Array.from(
                                    new Array(numPages),
                                    (_, index) => (

                                        <div
                                            key={index}
                                            className="mb-4 shadow bg-white p-2 rounded"
                                        >

                                            <Page
                                                pageNumber={index + 1}
                                                scale={scale}
                                                renderAnnotationLayer={false}
                                                renderTextLayer={true}
                                            />

                                        </div>

                                    )
                                )}

                            </Document>

                        </div>

                    </div>

                </div>

            </div>

        </div>

    );

}